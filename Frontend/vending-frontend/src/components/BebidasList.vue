<template>
  <div>
    <h2>Refrescos Disponibles</h2>
    <table>
      <thead>
        <tr>
          <th>Nombre</th>
          <th>Precio</th>
          <th>Stock</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="bebida in bebidas" :key="bebida.nombre">
          <td>{{ bebida.nombre }}</td>
          <td>₡{{ bebida.precio }}</td>
          <td>{{ bebida.cantidad }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import { obtenerBebidas } from '../services/api';

export default {
  name: 'BebidasList',
  data() {
    return {
      bebidas: [],
    };
  },
  mounted() {
    obtenerBebidas()
      .then((res) => {
        this.bebidas = res.data;
        console.log('Bebidas cargadas:', this.bebidas);
      })
      .catch((err) => {
        console.error('Error al cargar las bebidas:', err);
      });
  },
};
</script>

<style scoped>
table {
  width: 100%;
  border-collapse: collapse;
}
th, td {
  padding: 8px;
  border: 1px solid #ccc;
}
</style>