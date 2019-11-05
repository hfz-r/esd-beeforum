<template>
  <div class="home">
    <section class="hero is-dark is-medium has-background">
      <img class="hero-background" src="../assets/transparent-bumblebee.png" alt="Background" />
      <div class="hero-body">
        <div class="container level">
          <div class="level-left">
            <div class="level-item">
              <div>
                <p class="title is-1 is-spaced is-family-monospace">
                  <span class="has-text-weight-semibold" style="color:yellow">bee</span>
                  <span class="has-text-black has-text-weight-bold p-l-xs">forum</span>
                </p>
                <p class="subtitle">a place to share thoughs!</p>
              </div>
            </div>
            <div class="level-item">
              <img src="../assets/spelling-bee.png" alt="Banner" width="100" />
            </div>
          </div>
        </div>
      </div>
    </section>
    <section class="section container">
      <div class="columns">
        <div class="column is-9">
          <div class="tabs is-medium">
            <ul>
              <router-link
                v-if="isAuthenticated"
                :to="{ name: 'home-my-feed' }"
                active-class="is-active"
                tag="li"
              >
                <a>
                  <span class="icon is-small">
                    <i class="fas fa-street-view" aria-hidden="true"></i>
                  </span>
                  <span>Your Feed</span>
                </a>
              </router-link>
              <router-link :to="{ name: 'home' }" exact active-class="is-active" tag="li">
                <a>
                  <span class="icon is-small">
                    <i class="fas fa-globe" aria-hidden="true"></i>
                  </span>
                  <span>Global Feed</span>
                </a>
              </router-link>
            </ul>
          </div>
          <router-view></router-view>
        </div>
        <div class="column is-3">
          <button
            v-show="isAuthenticated"
            class="button is-link is-fullwidth m-b-md"
            @click.prevent="initEditor"
          >New article</button>
          <div class="sidebar">
            <p class="is-family-monospace">Popular tags</p>
            <Tag class="tags are-medium" shape="is-rounded is-dark" :tags="tags" />
          </div>
        </div>
      </div>
      <input-editor v-model="editorValue" :show="editor.show" :loading="editor.loading" />
    </section>
  </div>
</template>

<script>
import { mapState, mapGetters } from "vuex";
import { FETCH_TAGS, ARTICLE_RESET_STATE } from "@/store/actions.type";
import { SET_EDITOR_STATE, SET_EDITOR_VALUE } from "@/store/mutations.type";
import Tag from "@/components/Tag";
import InputEditor from "@/components/InputEditor";

export default {
  name: "home",
  components: { Tag, InputEditor },
  mounted() {
    this.$store.dispatch(FETCH_TAGS);
  },
  computed: {
    ...mapState({ editor: state => state.article.editor }),
    ...mapGetters(["isAuthenticated", "tags"]),
    editorValue: {
      get() {
        return this.editor.value;
      },
      set(value) {
        this.$store.commit(SET_EDITOR_VALUE, value);
      }
    }
  },
  methods: {
    initEditor() {
      this.$store.dispatch(ARTICLE_RESET_STATE);
      this.$store.commit(SET_EDITOR_STATE, true);
    }
  }
};
</script>

<style lang="scss" scoped>
.home .sidebar {
  padding: 5px 10px 10px;
  background: whitesmoke;
  border-radius: 4px;

  p {
    margin-bottom: 0.2rem;
  }
}

.hero.has-background {
  position: relative;
  overflow: hidden;
}

.hero-background {
  position: absolute;
  object-fit: contain;
  object-position: right center;
  width: 100%;
  height: 100%;
  padding-bottom: 30px;

  &.is-transparent {
    opacity: 0.3;
  }
}
</style>
