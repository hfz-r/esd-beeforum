import Vue from 'vue';
import Router from 'vue-router';

Vue.use(Router);

export default new Router({
  routes: [
    {
      path: '/',
      component: () => import('@/views/Home'),
      children: [
        {
          path: '',
          name: 'home',
          component: () => import('@/views/HomeGlobal')
        },
        {
          path: 'my-feed',
          name: 'home-my-feed',
          component: () => import('@/views/HomeMyFeed')
        },
        {
          path: 'tag/:tag',
          name: 'home-tag'
          // component: () => import('@/views/HomeTag')
        }
      ]
    },
    {
      path: '/about',
      name: 'about',
      component: () => import('@/views/About')
    }
  ]
});
