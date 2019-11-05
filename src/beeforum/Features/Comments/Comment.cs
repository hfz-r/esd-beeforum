using System.Collections.Generic;

namespace beeforum.Features.Comments
{
    public class CommentEnvelope
    {
        public CommentEnvelope(Domain.Comment comment)
        {
            Comment = comment;
        }

        public Domain.Comment Comment { get; }
    }

    public class CommentsEnvelope
    {
        public CommentsEnvelope(List<Domain.Comment> comments)
        {
            Comments = comments;
        }

        public List<Domain.Comment> Comments { get; }
    }
}