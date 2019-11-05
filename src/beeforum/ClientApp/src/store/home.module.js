import { TagsService, ArticlesService } from '@/common/api.service';
import { FETCH_ARTICLES, FETCH_TAGS } from './actions.type';
import {
  FETCH_START,
  FETCH_END,
  SET_TAGS,
  UPDATE_ARTICLE_IN_LIST
} from './mutations.type';

const state = {
  tags: [],
  isLoading: true,
  articles: [],
  articlesCount: 0
};

const getters = {
  tags(state) {
    return state.tags;
  },

  isLoading(state) {
    return state.isLoading;
  },

  articles(state) {
    return state.articles;
  },

  articlesCount(state) {
    return state.articlesCount;
  }
};

const actions = {
  [FETCH_ARTICLES]({ commit }, params) {
    commit(FETCH_START);
    return ArticlesService.query(params.type, params.filters)
      .then(({ data }) => {
        commit(FETCH_END, data);
      })
      .catch(error => {
        throw new Error(error);
      });
  },

  [FETCH_TAGS]({ commit }) {
    return TagsService.get()
      .then(({ data }) => {
        commit(SET_TAGS, data.tags);
      })
      .catch(error => {
        throw new Error(error);
      });
  }
};

/* eslint no-param-reassign: ["error", { "props": false }] */
const mutations = {
  [FETCH_START](state) {
    state.isLoading = true;
  },

  [FETCH_END](state, { articles, articlesCount }) {
    state.articles = articles;
    state.articlesCount = articlesCount;
    state.isLoading = false;
  },

  [SET_TAGS](state, tags) {
    state.tags = tags;
  },

  [UPDATE_ARTICLE_IN_LIST](state, data) {
    state.articles = state.articles.map(article => {
      if (article.slug !== data.slug) {
        return article;
      }
      article.favorited = data.favorited;
      article.favoritesCount = data.favoritesCount;
      return article;
    });
  }
};

export default {
  state,
  getters,
  actions,
  mutations
};
