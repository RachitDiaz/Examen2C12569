import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5012/api/Vending',
});

export const obtenerBebidas = () => api.get('/bebidas');

export const realizarCompra = (compra) => api.post('/comprar', compra);