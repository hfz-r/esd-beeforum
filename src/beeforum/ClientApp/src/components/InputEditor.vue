<template>
  <div
    class="editor editor--float"
    :class="{'editor--hidden': !show, 'editor--focus-input': focusInput}"
  >
    <div class="editor__overlay" :class="{'editor__overlay--show' : loading}">
      <pulse-loader :color="'#363636'" />
    </div>

    <div class="editor__close editor__format_button">&times;</div>

    <!-- body -->
    <div class="columns">
      <div class="column m-b-xxl"></div>
    </div>

    <div class="editor__submit_bar">
      <button class="button button--thin_text">Submit</button>
    </div>
  </div>
</template>

<script>
import PulseLoader from "@/components/PulseLoader";

export default {
  name: "InputEditor",
  components: { PulseLoader },
  props: ["show", "loading", "value"],
  data() {
    return {
      tagInput: null,
      inProgress: false,
      errors: {},
      showTab: 0,
      mentions: [],
      focusInput: false
    };
  }
};
</script>

<style lang="scss" scoped>
@import "@/sass/_variables.scss";

.editor {
  width: 35rem;
  border: 0.125rem solid $grey-light;
  border-bottom: none;
  border-radius: 0.25rem 0.25rem 0 0;
  margin-bottom: 0;
  pointer-events: all;
  transition: margin-bottom 0.2s, filter 0.2s, border-color 0.2s;
  outline: none;
  position: fixed;

  z-index: 2;
  bottom: 0;

  @at-root #{&}--focus-input {
    border-color: $grey-darker;
  }

  @at-root #{&}--hidden {
    pointer-events: none;
    opacity: 0;
    margin-bottom: -3rem;
    transition: margin-bottom 0.2s, opacity 0.2s;
  }

  @at-root #{&}__overlay {
    position: absolute;
    left: 0;
    top: 0;
    height: 100%;
    width: 100%;
    z-index: 5;
    background-color: rgba(0, 0, 0, 0.15);
    display: flex;
    align-items: center;
    justify-content: center;
    pointer-events: none;
    opacity: 0;

    transition: all 0.2s;

    @at-root #{&}--show {
      pointer-events: all;
      opacity: 1;
    }
  }

  @at-root #{&}__close {
    position: absolute;
    right: 0.3rem;
    top: 0.5rem;
    height: 1.5rem;
    width: 1.5rem;
    text-align: center;
    line-height: 1.4rem;
    cursor: pointer;
    @include user-select(none);
    @include text($family-sans-serif, 1rem, 600);
    color: $black-ter;
    border: thin solid $grey;
    border-radius: 0.25rem;
    transition: background-color 0.2s;
    margin: 0;

    &:hover {
      background-color: $grey-dark;
    }
    &:active {
      background-color: $grey-darker;
    }
  }

  @at-root #{&}__reply_username {
    position: absolute;
    width: 100%;
    text-align: center;
    top: 0.5rem;
  }

  @at-root #{&}__submit_bar {
    display: flex;
    justify-content: flex-end;
    height: 2rem;
    align-items: center;
    padding-right: 0.3rem;
    background-color: $grey-lighter;

    button {
      font-size: 0.8rem;
      height: 1.5rem;
      padding: 0 0.25rem;
      border-radius: 3px;
      border-color: $grey-darker;
    }
  }
}

@media (max-width: 420px) {
  .editor {
    width: 100%;
    left: 0;

    @at-root #{&}__reply_username {
      top: auto;
      bottom: 0.5rem;
      left: 0.5rem;
      width: auto;
    }
  }
}
</style>