import Vue from 'vue';
import { ARTICLE_RESET_STATE } from './actions.type';
import {
  RESET_STATE,
  SET_EDITOR_STATE,
  SET_EDITOR_VALUE
} from './mutations.type';

const defaultState = {
  article: {
    author: {},
    title: '',
    description: '',
    body: '',
    tagList: []
  },
  comments: [],
  editor: {
    show: false,
    loading: false,
    value: ''
  }
};

const state = { ...defaultState };

const getters = {
  article(state) {
    return state.article;
  },

  comments(state) {
    return state.comments;
  }
};

const actions = {
  [ARTICLE_RESET_STATE]({ commit }) {
    commit(RESET_STATE);
  }
};

const mutations = {
  [RESET_STATE](state) {
    for (let s in state) {
      Vue.set(state, s, defaultState[s]);
    }
  },

  [SET_EDITOR_STATE](state, value) {
    state.editor.show = value;
  },

  [SET_EDITOR_VALUE](state, value) {
    state.editor.value = value;
  }
};

export default {
  state,
  actions,
  mutations,
  getters
};
