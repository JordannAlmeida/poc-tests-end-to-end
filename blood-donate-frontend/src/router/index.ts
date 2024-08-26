import { createRouter, createWebHistory } from 'vue-router';
import Home from '@/views/Home.vue';
import RegisterUser from '@/views/RegisterUser.vue';
import BloodStock from '@/views/BloodStock.vue';

const routes = [
  { path: '/', component: Home },
  { path: '/register', component: RegisterUser },
  { path: '/stock', component: BloodStock },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;