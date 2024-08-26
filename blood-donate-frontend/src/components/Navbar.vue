<template>
    <nav>
      <router-link to="/">Home</router-link>
      <router-link to="/register">Register User</router-link>
      <router-link to="/stock">Blood Stock</router-link>
    </nav>
  </template>
  
  <script lang="ts">
  import { defineComponent } from 'vue';
  
  export default defineComponent({
    name: 'NavbarComponent',
  });
  </script>
  
  <style scoped>
  nav {
    display: flex;
    gap: 10px;
  }
  </style>