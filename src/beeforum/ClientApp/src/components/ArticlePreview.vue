<template>
  <article class="media">
    <figure class="media-left">
      <p class="image is-48x48">
        <router-link :to="{name:'profile', params:{username:article.author.username}}">
          <img class="is-rounded" :src="article.author.image" />
        </router-link>
      </p>
    </figure>
    <div class="media-content">
      <div class="content">
        <p>
          <router-link :to="{name:'profile', params:{username:article.author.username}}">
            <strong>{{article.author.username}}</strong>
          </router-link>
          <small class="p-l-sm">{{article.createdAt | date}}</small>
          <br />
          <router-link :to="{name: 'article', params:{slug: article.slug}}">
            <strong class="title">{{article.title}}</strong>
            <br />
            <small class="is-family-monospace has-text-grey">{{article.description}}</small>
          </router-link>
        </p>
      </div>
      <ArticleMeta isPreview :article="article" />
    </div>
    <div class="media-right">
      <button class="delete"></button>
    </div>
  </article>
</template>

<script>
import ArticleMeta from "./ArticleMeta";

export default {
  name: "ArticlePreview",
  components: {
    ArticleMeta
  },
  props: {
    article: {
      type: Object,
      required: true
    }
  }
};
</script>
