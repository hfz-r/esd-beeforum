<template>
  <div>
    <div v-if="isLoading" class="article-preview">
      <PulseLoader :color="'#363636'" />
    </div>
    <div v-else>
      <div v-if="articles.length === 0" class="article-preview">No articles are here.</div>
      <div v-else class="box content">
        <ArticlePreview
          v-for="(article, index) in articles"
          :key="article.title + index"
          :article="article"
        />
      </div>
    </div>
  </div>
</template>

<script>
import { mapGetters } from "vuex";
import { FETCH_ARTICLES } from "@/store/actions.type";
import PulseLoader from "@/components/PulseLoader";
import ArticlePreview from "@/components/ArticlePreview";

export default {
  name: "ArticleList",
  components: {
    PulseLoader,
    ArticlePreview
  },
  props: {
    type: {
      type: String,
      required: false,
      default: "all"
    },
    author: {
      type: String,
      required: false
    },
    tag: {
      type: String,
      required: false
    },
    favorited: {
      type: String,
      required: false
    },
    itemsPerPage: {
      type: Number,
      required: false,
      default: 10
    }
  },
  data() {
    return {
      currentPage: 1
    };
  },
  computed: {
    listConfig() {
      const { type } = this;
      const filters = {
        offset: (this.currentPage - 1) * this.itemsPerPage,
        limit: this.itemsPerPage
      };
      if (this.author) {
        filters.author = this.author;
      }
      if (this.tag) {
        filters.tag = this.tag;
      }
      if (this.favorited) {
        filters.favorited = this.favorited;
      }
      return {
        type,
        filters
      };
    },
    pages() {
      if (this.isLoading || this.articlesCount <= this.itemsPerPage) {
        return [];
      }
      return [
        ...Array(Math.ceil(this.articlesCount / this.itemsPerPage)).keys()
      ].map(pg => pg + 1);
    },
    ...mapGetters(["isLoading", "articles", "articlesCount"])
  },
  watch: {
    currentPage(newValue) {
      this.listConfig.filters.offset = (newValue - 1) * this.itemsPerPage;
      this.fetchArticles();
    },
    type() {
      this.resetPagination();
      this.fetchArticles();
    },
    author() {
      this.resetPagination();
      this.fetchArticles();
    },
    tag() {
      this.resetPagination();
      this.fetchArticles();
    },
    favorited() {
      this.resetPagination();
      this.fetchArticles();
    }
  },
  mounted() {
    this.fetchArticles();
  },
  methods: {
    fetchArticles() {
      this.$store.dispatch(FETCH_ARTICLES, this.listConfig);
    },
    resetPagination() {
      this.listConfig.offset = 0;
      this.currentPage = 1;
    }
  }
};
</script>
