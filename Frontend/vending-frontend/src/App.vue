<template>
  <div id="app">
    <h1>Máquina Expendedora de Refrescos</h1>
    <BebidasList />
  </div>
</template>

<script>
import BebidasList from './components/BebidasList.vue';

export default {
  name: 'App',
  components: {
    BebidasList,
  },
};
</script>