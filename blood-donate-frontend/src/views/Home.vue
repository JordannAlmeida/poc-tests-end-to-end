<template>
    <div>
      <Navbar />
      <h1>Welcome to BloodDonate</h1>
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent } from 'vue';
  import Navbar from '../components/Navbar.vue';
  
  export default defineComponent({
    name: 'HomeComponent',
    components: {
      Navbar,
    },
  });
  </script>