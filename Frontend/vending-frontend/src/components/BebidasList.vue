<template>
  <div>
    <h2>Refrescos Disponibles</h2>

    <table>
      <thead>
        <tr>
          <th>Nombre</th>
          <th>Precio</th>
          <th>Stock</th>
          <th>Cantidad</th>
          <th>Subtotal</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(bebida, index) in bebidas" :key="bebida.nombre">
          <td>{{ bebida.nombre }}</td>
          <td>₡{{ bebida.precio }}</td>
          <td>{{ bebida.cantidad }}</td>
          <td>
            <input
              type="number"
              min="0"
              :max="bebida.cantidad"
              v-model.number="cantidades[index]"
              @input="actualizarTotal"
            />
          </td>
          <td>₡{{ subtotal(index) }}</td>
        </tr>
      </tbody>
    </table>

    <p><strong>Total a pagar:</strong> ₡{{ total }}</p>
    <p v-if="mensajeError" style="color: red">{{ mensajeError }}</p>
  </div>
</template>

<script>
import { obtenerBebidas } from '../services/api';

export default {
  name: 'BebidasList',
  data() {
    return {
      bebidas: [],
      cantidades: [],
      total: 0,
      mensajeError: ''
    };
  },
  methods: {
    subtotal(index) {
      const cantidad = this.cantidades[index] || 0;
      const bebida = this.bebidas[index];
      return bebida ? bebida.precio * cantidad : 0;
    },
    actualizarTotal() {
      this.total = 0;
      this.mensajeError = '';

      this.bebidas.forEach((bebida, index) => {
        const cantidad = this.cantidades[index] || 0;

        if (cantidad > bebida.cantidad) {
          this.mensajeError = `No hay suficiente stock de ${bebida.nombre}`;
        }

        this.total += bebida.precio * cantidad;
      });
    }
  },
  mounted() {
    obtenerBebidas()
      .then((res) => {
        this.bebidas = res.data;
        this.cantidades = new Array(this.bebidas.length).fill(0);
      })
      .catch(() => {
        this.mensajeError = 'Error al cargar bebidas';
      });
  }
};
</script>

<style scoped>
input {
  width: 60px;
}
</style>
