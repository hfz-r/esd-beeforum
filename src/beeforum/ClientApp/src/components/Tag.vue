<template>
  <div class="tags">
    <span v-for="(tag, index) in tags" :key="tag + index">
      <router-link :to="{name: 'home-tag', params:{tag: tag}}">
        <span class="tag" :class="shape">{{tag}}</span>
      </router-link>
    </span>
  </div>
</template>

<script>
export default {
  name: "Tag",
  props: {
    tags: Array,
    shape: String
  }
};
</script>

<style>
</style>