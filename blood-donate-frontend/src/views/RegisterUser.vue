<template>
    <div>
      <Navbar />
      <h1>Register User</h1>
      <form @submit.prevent="registerUser">
        <div>
          <label for="name">Name:</label>
          <input type="text" v-model="name" required />
        </div>
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
        <div>
          <label for="quantity">Quantity (ml):</label>
          <input type="number" v-model="quantity" required />
        </div>
        <button type="submit">Register</button>
      </form>
    </div>
  </template>
  
  <script lang="ts">
  import { defineComponent } from 'vue';
  import Navbar from '../components/Navbar.vue';
  import apiClient from '../services/api';
  
  export default defineComponent({
    name: 'RegisterUser',
    components: {
      Navbar,
    },
    data() {
      return {
        name: '',
        bloodType: '',
        rhFactor: '',
        quantity: 0,
      };
    },
    methods: {
      async registerUser() {
        try {
          await apiClient.post('/api/register', {
            name: this.name,
            bloodType: this.bloodType,
            rhFactor: this.rhFactor,
            quantity: this.quantity,
          });
          alert('User registered successfully');
        } catch (error) {
          console.error(error);
          alert('Failed to register user');
        }
      },
    },
  });
  </script>