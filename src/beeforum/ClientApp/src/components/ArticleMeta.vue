<template>
  <nav class="level is-mobile">
    <div class="level-left">
      <a class="level-item">
        <span class="icon is-small has-text-grey-light">
          <i class="fas fa-share"></i>
        </span>
      </a>
      <a class="level-item">
        <span class="icon is-small has-text-grey-light">
          <i class="fa fa-comments"></i>
        </span>
      </a>
      <a class="level-item">
        <span class="icon is-small has-text-grey-light">
          <i class="fas fa-heart"></i>
        </span>
      </a>
    </div>
    <div class="level-right">
      <Tag class="tags" shape="is-warning" :tags="article.tagList" />
    </div>
  </nav>
</template>

<script>
import Tag from "./Tag";

export default {
  name: "ArticleMeta",
  components: { Tag },
  props: {
    article: {
      type: Object,
      required: true
    },
    actions: {
      type: Boolean,
      required: false,
      default: false
    }
  }
};
</script>
