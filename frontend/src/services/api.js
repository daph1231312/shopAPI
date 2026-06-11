const BASE = '/api';

const handle = async (res) => {
  if (!res.ok) {
    const err = await res.json().catch(() => ({ error: 'Unknown error' }));
    throw new Error(err.error || `HTTP ${res.status}`);
  }
  return res.status === 204 ? null : res.json();
};

export const getProducts = (categoryId) =>
  fetch(`${BASE}/products${categoryId ? `?categoryId=${categoryId}` : ''}`).then(handle);
export const createProduct = (data) =>
  fetch(`${BASE}/products`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) }).then(handle);
export const updateProduct = (id, data) =>
  fetch(`${BASE}/products/${id}`, { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) }).then(handle);
export const deleteProduct = (id) =>
  fetch(`${BASE}/products/${id}`, { method: 'DELETE' }).then(handle);

export const getCategories = () => fetch(`${BASE}/categories`).then(handle);
export const createCategory = (data) =>
  fetch(`${BASE}/categories`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) }).then(handle);
export const deleteCategory = (id) =>
  fetch(`${BASE}/categories/${id}`, { method: 'DELETE' }).then(handle);

export const getOrders = () => fetch(`${BASE}/orders`).then(handle);
export const createOrder = (data) =>
  fetch(`${BASE}/orders`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) }).then(handle);
export const updateOrderStatus = (id, status) =>
  fetch(`${BASE}/orders/${id}/status`, { method: 'PATCH', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ status }) }).then(handle);
