<template>
    <div>
      <Navbar />
      <h1>Blood Stock</h1>
      <form @submit.prevent="getBloodStock">
        <div>
          <label for="bloodType">Blood Type:</label>
          <select v-model="bloodType" required>
            <option>A</option>
            <option>B</option>
            <option>AB</option>
            <option>O</option>
          </select>
        </div>
        <div>
          <label for="rhFactor">RH Factor:</label>
          <select v-model="rhFactor" required>
            <option>+</option>
            <option>-</option>
          </select>
        </div>
        <button type="submit">Get Stock</button>
      </form>
      <div v-if="stock !== null">
        <h2>Total Stock: {{ stock }} ml</h2>
      </div>
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent } from 'vue';
  import Navbar from '../components/Navbar.vue';
  import apiClient from '../services/api';
  
  export default defineComponent({
    name: 'BloodStock',
    components: {
      Navbar,
    },
    data() {
      return {
        bloodType: '',
        rhFactor: '',
        stock: null,
      };
    },
    methods: {
      async getBloodStock() {
        try {
          const response = await apiClient.get('/api/stock', {
            params: {
              bloodType: this.bloodType,
              rhFactor: this.rhFactor,
            },
          });
          this.stock = response.data.stock;
        } catch (error) {
          console.error(error);
          alert('Failed to get blood stock');
        }
      },
    },
  });
  </script>