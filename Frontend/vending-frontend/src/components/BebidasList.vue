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
                        <input type="number" min="0" :max="bebida.cantidad" v-model.number="cantidades[index]"
                            @input="actualizarTotal" />
                    </td>
                    <td>₡{{ subtotal(index) }}</td>
                </tr>
            </tbody>
        </table>

        <p><strong>Total a pagar:</strong> ₡{{ total }}</p>
        <h3>Ingreso de Dinero</h3>
        <div class="money-inputs">
            <label>₡1000: <input type="number" min="0" v-model.number="dinero['1000']" /></label>
            <label>₡500: <input type="number" min="0" v-model.number="dinero['500']" /></label>
            <label>₡100: <input type="number" min="0" v-model.number="dinero['100']" /></label>
            <label>₡50: <input type="number" min="0" v-model.number="dinero['50']" /></label>
            <label>₡25: <input type="number" min="0" v-model.number="dinero['25']" /></label>
        </div>

        <p><strong>Dinero ingresado:</strong> ₡{{ totalIngresado }}</p>

        <button @click="realizarCompra">Realizar compra</button>
        <p v-if="mensajeError" style="color: red">{{ mensajeError }}</p>
    </div>
</template>

<script>

import { obtenerBebidas, realizarCompra } from '../services/api';

export default {
    name: 'BebidasList',
    data() {
        return {
            bebidas: [],
            cantidades: [],
            total: 0,
            mensajeError: '',
            dinero: {
                '1000': 0,
                '500': 0,
                '100': 0,
                '50': 0,
                '25': 0
            }
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
        },
        realizarCompra() {
            this.mensajeError = '';

            let indexSeleccionado = -1;
            for (let i = 0; i < this.cantidades.length; i++) {
                if (this.cantidades[i] > 0) {
                    indexSeleccionado = i;
                    break;
                }
            }

            if (indexSeleccionado === -1) {
                this.mensajeError = 'Seleccione al menos un refresco.';
                return;
            }

            const bebida = this.bebidas[indexSeleccionado];
            const cantidad = this.cantidades[indexSeleccionado];

            if (cantidad > bebida.cantidad) {
                this.mensajeError = 'No hay suficiente stock disponible.';
                return;
            }

            const totalCompra = bebida.precio * cantidad;
            let totalIngresado = 0;
            if (this.dinero["1000"]) totalIngresado += 1000 * this.dinero["1000"];
            if (this.dinero["500"]) totalIngresado += 500 * this.dinero["500"];
            if (this.dinero["100"]) totalIngresado += 100 * this.dinero["100"];
            if (this.dinero["50"]) totalIngresado += 50 * this.dinero["50"];
            if (this.dinero["25"]) totalIngresado += 25 * this.dinero["25"];

            if (totalIngresado < totalCompra) {
                this.mensajeError = 'Dinero insuficiente para completar la compra.';
                return;
            }

            const compra = {
                nombreBebida: bebida.nombre,
                cantidad: cantidad,
                dineroIngresado: {
                    1000: this.dinero["1000"],
                    500: this.dinero["500"],
                    100: this.dinero["100"],
                    50: this.dinero["50"],
                    25: this.dinero["25"]
                }
            };

            realizarCompra(compra).then(response => {
                const data = response.data;
                if (!data.exito) {
                    this.mensajeError = data.mensaje;
                } else {
                    alert("Compra realizada con éxito. Vuelto: ₡" + data.cambio);
                    this.obtenerBebidas();
                    for (let i = 0; i < this.cantidades.length; i++) {
                        this.cantidades[i] = 0;
                    }
                    for (const key in this.dinero) {
                        this.dinero[key] = 0;
                    }
                }
            }).catch(error => {
                this.mensajeError = "Error al procesar la compra.";
                console.log(error);
            });
        },
        obtenerBebidas() {
            obtenerBebidas().then((res) => {
                this.bebidas = res.data;
                this.cantidades = new Array(this.bebidas.length).fill(0);
            }).catch(() => {
                this.mensajeError = 'Error al cargar bebidas';
            });
        }
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
    mounted() {
        this.obtenerBebidas();
    }
};
</script>

<style scoped>
input {
    width: 60px;
}
</style>
