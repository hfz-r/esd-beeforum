import Vue from 'vue';
import App from './App.vue';
import router from './router';
import store from './store';
import DateFilter from './common/date.filter';
import ApiService from './common/api.service';
import { CHECK_AUTH } from './store/actions.type';
import './register-sw';
import './sass/app.scss';

Vue.config.productionTip = false;
Vue.filter('date', DateFilter);

ApiService.init('http://localhost:10808/api');

router.beforeEach((to, from, next) =>
  Promise.all([store.dispatch(CHECK_AUTH)]).then(next)
);

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app');
