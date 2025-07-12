<template>
  <div class="main-container">
    <section class="section">
      <h2>Refrescos Disponibles</h2>
      <div v-for="(bebida, index) in bebidas" :key="bebida.nombre" class="card">
        <h3>{{ bebida.nombre }}</h3>
        <p>₡{{ bebida.precio }}</p>
        <p>Stock: {{ bebida.cantidad }} unidades</p>
        <div class="quantity">
          <button @click="cambiarCantidad(index, -1)">-</button>
          <input type="number" min="0" :max="bebida.cantidad" v-model.number="cantidades[index]">
          <button @click="cambiarCantidad(index, 1)">+</button>
        </div>
        <p>₡{{ subtotal(index) }}</p>
      </div>
      <p class="total">Total: ₡{{ total }}</p>
    </section>

    <section class="section">
      <h2>Sistema de Pago</h2>
      <div class="money-inputs">
        <h4>Billetes</h4>
        <label>₡1000: <input type="number" min="0" v-model.number="dinero['1000']" /></label>
        <h4>Monedas</h4>
        <label>₡500: <input type="number" min="0" v-model.number="dinero['500']" /></label>
        <label>₡100: <input type="number" min="0" v-model.number="dinero['100']" /></label>
        <label>₡50: <input type="number" min="0" v-model.number="dinero['50']" /></label>
        <label>₡25: <input type="number" min="0" v-model.number="dinero['25']" /></label>
      </div>

      <p class="summary"><strong>Dinero insertado:</strong> ₡{{ totalIngresado }}</p>
      <p class="summary"><strong>Total a pagar:</strong> ₡{{ total }}</p>
      <p class="summary"><strong>Cambio:</strong> ₡{{ cambio }}</p>

      <button :disabled="total === 0 || total > totalIngresado" @click="realizarCompra">Realizar Compra</button>
      <button @click="cancelarCompra">Cancelar</button>

      <p v-if="mensajeError" class="error">{{ mensajeError }}</p>
    </section>

    <section v-if="Object.keys(desgloseCambio).length > 0" class="section">
      <h3>Vuelto Entregado</h3>
      <table>
        <thead>
          <tr>
            <th>Moneda</th>
            <th>Cantidad</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(cantidad, valor) in desgloseCambio" :key="valor">
            <td>₡{{ valor }}</td>
            <td>{{ cantidad }}</td>
          </tr>
        </tbody>
      </table>
    </section>

    <footer>
      <h3>Estado de Monedas para Cambio</h3>
      <div class="monedas-estado">
        <span>₡500<br>{{ estadoCambio["500"] }} monedas</span>
        <span>₡100<br>{{ estadoCambio["100"] }} monedas</span>
        <span>₡50<br>{{ estadoCambio["50"] }} monedas</span>
        <span>₡25<br>{{ estadoCambio["25"] }} monedas</span>
      </div>
    </footer>
  </div>
</template>

<script>
import { obtenerBebidas, realizarCompra, obtenerEstadoCambio } from '../services/api';

export default {
  name: 'BebidasList',
  data() {
    return {
      bebidas: [],
      cantidades: [],
      total: 0,
      mensajeError: '',
      cambio: 0,
      desgloseCambio: {},
      estadoCambio: {
        "500": 0,
        "100": 0,
        "50": 0,
        "25": 0
      },
      dinero: {
        '1000': 0,
        '500': 0,
        '100': 0,
        '50': 0,
        '25': 0
      }
    };
  },
  computed: {
    totalIngresado() {
      let total = 0;
      for (const key in this.dinero) {
        total += parseInt(key) * this.dinero[key];
      }
      return total;
    }
  },
  methods: {
    subtotal(index) {
      const cantidad = this.cantidades[index] || 0;
      return this.bebidas[index].precio * cantidad;
    },
    cambiarCantidad(index, delta) {
      const nueva = (this.cantidades[index] || 0) + delta;
      if (nueva >= 0 && nueva <= this.bebidas[index].cantidad) {
        this.cantidades[index] = nueva;
        this.actualizarTotal();
      }
    },
    actualizarTotal() {
      let nuevoTotal = 0;
      for (let i = 0; i < this.bebidas.length; i++) {
        const cantidad = this.cantidades[i] || 0;
        nuevoTotal += this.bebidas[i].precio * cantidad;
      }
      this.total = nuevoTotal;
    },
    realizarCompra() {
      this.mensajeError = '';
      const index = this.cantidades.findIndex(c => c > 0);
      if (index === -1) {
        this.mensajeError = 'Seleccione una bebida';
        return;
      }

      const compra = {
        nombreBebida: this.bebidas[index].nombre,
        cantidad: this.cantidades[index],
        dineroIngresado: { ...this.dinero }
      };

      realizarCompra(compra)
        .then(res => {
          if (!res.data.exito) {
            this.mensajeError = res.data.mensaje;
          } else {
            this.cambio = res.data.cambio;
            this.desgloseCambio = res.data.desgloseCambio;
            alert('Compra realizada con éxito');
            this.obtenerBebidas();
            this.obtenerEstadoCambio();
            this.cantidades = new Array(this.bebidas.length).fill(0);
            for (let key in this.dinero) this.dinero[key] = 0;
            this.actualizarTotal();
          }
        })
        .catch(() => {
          this.mensajeError = 'Error al procesar la compra';
        });
    },
    cancelarCompra() {
      for (let i = 0; i < this.cantidades.length; i++) {
        this.cantidades[i] = 0;
      }
      for (let key in this.dinero) {
        this.dinero[key] = 0;
      }
      this.actualizarTotal();
      this.mensajeError = '';
      this.cambio = 0;
      this.desgloseCambio = {};
    },
    obtenerBebidas() {
      obtenerBebidas()
        .then(res => {
          this.bebidas = res.data;
          this.cantidades = new Array(this.bebidas.length).fill(0);
          this.actualizarTotal();
        })
        .catch(() => {
          this.mensajeError = 'Error al cargar bebidas';
        });
    },
    obtenerEstadoCambio() {
      obtenerEstadoCambio()
        .then(res => {
          this.estadoCambio = res.data;
        })
        .catch(() => {
          this.mensajeError = 'Error al obtener estado de monedas';
        });
    }
  },
  mounted() {
    this.obtenerBebidas();
    this.obtenerEstadoCambio();
  }
};
</script>
<style scoped>
.main-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 1rem;
  font-family: Arial, sans-serif;
}

.section {
  background-color: #f7f7f7;
  border-radius: 10px;
  padding: 1rem;
  margin: 1rem;
  box-shadow: 0 0 5px rgba(0,0,0,0.1);
}

.section h2 {
  font-size: 1.2rem;
  margin-bottom: 0.5rem;
  text-align: center;
}

.section h3 {
  font-size: 1rem;
  margin-bottom: 0.4rem;
}

.card {
  border: 1px solid #ccc;
  border-radius: 8px;
  padding: 0.6rem;
  text-align: center;
  background: white;
}

.card h3 {
  font-size: 1rem;
  margin: 0.2rem 0;
}

.card p {
  font-size: 0.9rem;
  margin: 0.1rem 0;
}

.quantity {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 0.3rem;
  margin: 0.5rem 0;
}

.quantity input {
  width: 50px;
  text-align: center;
}

.quantity button {
  width: 25px;
  height: 25px;
  font-weight: bold;
}

.money-inputs label {
  display: flex;
  justify-content: space-between;
  margin: 0.2rem 0;
}

.money-inputs input {
  width: 60px;
  text-align: right;
}

.summary {
  font-size: 0.9rem;
  margin-top: 0.3rem;
}

button {
  margin-top: 0.5rem;
  padding: 0.4rem 0.8rem;
  font-size: 0.9rem;
  border-radius: 4px;
  cursor: pointer;
}

button:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.error {
  color: red;
  font-size: 0.85rem;
  margin-top: 0.3rem;
}

footer {
  margin-top: 1.5rem;
  text-align: center;
  font-size: 0.85rem;
}

.monedas-estado {
  display: flex;
  justify-content: center;
  gap: 1rem;
  margin-top: 0.5rem;
}

.monedas-estado span {
  background-color: #eaeaea;
  padding: 0.5rem;
  border-radius: 6px;
  min-width: 60px;
  text-align: center;
}

table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.85rem;
}

table, th, td {
  border: 1px solid #ccc;
}

th, td {
  padding: 0.4rem;
  text-align: center;
}

.section:nth-child(1) {
  width: 700px;
}

.section:nth-child(1) > div {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 0.7rem;
}
.section:nth-child(2),
.section:nth-child(3) {
  width: 400px;
}

</style>