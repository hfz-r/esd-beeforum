<template>
  <div class="home-global">
    <ArticleList type="all" />
  </div>
</template>

<script>
import ArticleList from "@/components/ArticleList";

export default {
  name: "home-global",
  components: {
    ArticleList
  }
};
</script>
