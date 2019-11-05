using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using beeforum.Domain;
using beeforum.Infrastructure;
using beeforum.Infrastructure.Data;
using beeforum.Infrastructure.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace beeforum.Features.Articles
{
    public class Edit
    {
        public class ArticleData
        {
            public string Title { get; set; }

            public string Description { get; set; }

            public string Body { get; set; }

            public string[] TagList { get; set; }
        }

        public class Command : IRequest<ArticleEnvelope>
        {
            public ArticleData Article { get; set; }

            public string Slug { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Article).NotNull();
            }
        }

        #region Utilities

        /// <summary>
        /// get the list of Tags to be added
        /// </summary>
        private static async Task<IList<Tag>> GetTagsToCreate(IEnumerable<string> articleTagList, ApplicationDbContext context)
        {
            var tagsToCreate = new List<Tag>();
            foreach (var tag in articleTagList)
            {
                var t = await context.Tags.FindAsync(tag);
                if (t == null)
                {
                    t = new Tag {TagId = tag};

                    tagsToCreate.Add(t);
                }
            }

            return tagsToCreate;
        }

        /// <summary>
        /// check which article tags need to be added
        /// </summary>
        private static IList<ArticleTag> GetArticleTagsToCreate(Article article, IEnumerable<string> articleTagList)
        {
            return (from tag in articleTagList
                    let at = article.ArticleTags.FirstOrDefault(t => t.TagId == tag)
                    where at == null
                    select new ArticleTag
                    {
                        Article = article,
                        ArticleId = article.ArticleId,
                        Tag = new Tag {TagId = tag},
                        TagId = tag
                    })
                .ToList();
        }

        /// <summary>
        /// check which article tags need to be deleted
        /// </summary>
        private static IList<ArticleTag> GetArticleTagsToDelete(Article article, IEnumerable<string> articleTagList)
        {
            return (from tag in article.ArticleTags
                    let at = articleTagList.FirstOrDefault(t => t == tag.TagId)
                    where at == null
                    select tag)
                .ToList();
        }

        #endregion

        public class Handler : IRequestHandler<Command, ArticleEnvelope>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<ArticleEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var article = await _context.Articles
                    .Include(x => x.ArticleTags) // include also the article tags since they also need to be updated
                    .Where(x => x.Slug == message.Slug)
                    .FirstOrDefaultAsync(cancellationToken);
                if (article == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Article = Constants.NotFound});

                article.Description = message.Article.Description ?? article.Description;
                article.Body = message.Article.Body ?? article.Body;
                article.Title = message.Article.Title ?? article.Title;
                article.Slug = article.Title.GenerateSlug();

                // list of currently saved article tags for the given article
                var articleTagList = message.Article.TagList ?? new string[] { };

                var tagsToCreate = await GetTagsToCreate(articleTagList, _context);

                await _context.Tags.AddRangeAsync(tagsToCreate, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var articleTagsToCreate = GetArticleTagsToCreate(article, articleTagList);
                var articleTagsToDelete = GetArticleTagsToDelete(article, articleTagList);

                if (_context.ChangeTracker.Entries()
                        .First(x => x.Entity == article).State == EntityState.Modified ||
                    articleTagsToCreate.Any() ||
                    articleTagsToDelete.Any())
                {
                    article.UpdatedAt = DateTime.UtcNow;
                }

                // add the new article tags
                await _context.ArticleTags.AddRangeAsync(articleTagsToCreate, cancellationToken);
                // delete the tags that do not exist anymore
                _context.ArticleTags.RemoveRange(articleTagsToDelete);

                await _context.SaveChangesAsync(cancellationToken);

                return new ArticleEnvelope(await _context.Articles
                    .GetAllData()
                    .Where(x => x.Slug == article.Slug)
                    .FirstOrDefaultAsync(cancellationToken));
            }
        }
    }
}