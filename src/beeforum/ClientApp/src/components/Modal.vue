<template>
  <div class="modal is-active">
    <div class="modal-background"></div>
    <div class="modal-card">
      <header class="modal-card-head">
        <p class="modal-card-title">
          <slot name="title">default title</slot>
        </p>
        <button class="delete" aria-label="close" @click="onClose"></button>
      </header>
      <section class="modal-card-body">
        <slot name="body">default body</slot>
      </section>
      <footer class="modal-card-foot">
        <slot name="footer-button">default button</slot>
      </footer>
    </div>
  </div>
</template>

<script>
import { RESET_ERROR } from "@/store/actions.type";

export default {
  name: "Modal",
  methods: {
    onClose() {
      this.$store.dispatch(RESET_ERROR);
      this.$emit("close");
    }
  }
};
</script>

<style>
</style>