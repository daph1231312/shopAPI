import { useState, useEffect } from 'react'
import { getOrders, getProducts, createOrder, updateOrderStatus } from '../services/api'

const STATUSES = ['Pending', 'Confirmed', 'Shipped', 'Delivered', 'Cancelled']

export default function OrdersPage() {
  const [orders, setOrders] = useState([])
  const [products, setProducts] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [modal, setModal] = useState(false)
  const [expanded, setExpanded] = useState(null)
  const [form, setForm] = useState({ customerName: '', customerEmail: '', items: [{ productId: '', quantity: 1 }] })

  const load = async () => {
    try {
      setLoading(true)
      const [o, p] = await Promise.all([getOrders(), getProducts()])
      setOrders(o)
      setProducts(p)
      setError('')
    } catch (e) { setError(e.message) }
    finally { setLoading(false) }
  }

  useEffect(() => { load() }, [])

  const addItem = () => setForm(f => ({ ...f, items: [...f.items, { productId: '', quantity: 1 }] }))
  const removeItem = (i) => setForm(f => ({ ...f, items: f.items.filter((_, idx) => idx !== i) }))
  const updateItem = (i, field, val) => setForm(f => ({
    ...f, items: f.items.map((item, idx) => idx === i ? { ...item, [field]: val } : item)
  }))

  const handleCreate = async () => {
    try {
      await createOrder({
        customerName: form.customerName,
        customerEmail: form.customerEmail,
        items: form.items.map(i => ({ productId: parseInt(i.productId), quantity: parseInt(i.quantity) }))
      })
      setModal(false)
      setForm({ customerName: '', customerEmail: '', items: [{ productId: '', quantity: 1 }] })
      load()
    } catch (e) { setError(e.message) }
  }

  const handleStatus = async (id, status) => {
    try { await updateOrderStatus(id, status); load() } catch (e) { setError(e.message) }
  }

  const statusClass = (s) => `badge badge-${s.toLowerCase()}`

  return (
    <>
      <h1 className="page-title">Orders <span>{orders.length} total</span></h1>
      {error && <div className="error-msg">{error}</div>}

      <div className="card">
        <div className="card-header">
          <span className="card-title">All Orders</span>
          <button className="btn btn-primary" onClick={() => setModal(true)}>+ Place Order</button>
        </div>

        {loading ? <div className="loading">Loading…</div> : (
          <div className="table-wrap">
            <table>
              <thead>
                <tr><th>ID</th><th>Customer</th><th>Status</th><th>Total</th><th>Date</th><th>Actions</th></tr>
              </thead>
              <tbody>
                {orders.length === 0 ? (
                  <tr><td colSpan={6} className="empty-state">No orders yet.</td></tr>
                ) : orders.map(o => (
                  <>
                    <tr key={o.id} style={{ cursor: 'pointer' }} onClick={() => setExpanded(expanded === o.id ? null : o.id)}>
                      <td style={{ color: 'var(--muted)', fontSize: 12 }}>#{o.id}</td>
                      <td>
                        <strong>{o.customerName}</strong>
                        <br/><span style={{ fontSize: 12, color: 'var(--muted)' }}>{o.customerEmail}</span>
                      </td>
                      <td><span className={statusClass(o.status)}>{o.status}</span></td>
                      <td><strong>${o.total.toFixed(2)}</strong></td>
                      <td style={{ color: 'var(--muted)', fontSize: 12 }}>{new Date(o.createdAt).toLocaleDateString()}</td>
                      <td onClick={e => e.stopPropagation()}>
                        <select value={o.status} onChange={e => handleStatus(o.id, e.target.value)} style={{ fontSize: 12, padding: '4px 8px', width: 130 }}>
                          {STATUSES.map(s => <option key={s} value={s}>{s}</option>)}
                        </select>
                      </td>
                    </tr>
                    {expanded === o.id && (
                      <tr key={`${o.id}-items`}>
                        <td colSpan={6} style={{ background: 'var(--surface2)', padding: '12px 20px' }}>
                          <p style={{ fontSize: 12, color: 'var(--muted)', marginBottom: 8 }}>ORDER ITEMS</p>
                          {o.items.map((item, i) => (
                            <div key={i} style={{ display: 'flex', gap: 16, fontSize: 13, marginBottom: 4 }}>
                              <span>{item.productName}</span>
                              <span style={{ color: 'var(--muted)' }}>×{item.quantity}</span>
                              <span style={{ color: 'var(--accent2)' }}>${item.unitPrice.toFixed(2)} each</span>
                              <span>=&nbsp;<strong>${item.subtotal.toFixed(2)}</strong></span>
                            </div>
                          ))}
                        </td>
                      </tr>
                    )}
                  </>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {modal && (
        <div className="modal-overlay" onClick={() => setModal(false)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <div className="modal-title">Place New Order</div>
            <div className="form-grid">
              <div className="form-group">
                <label>Customer Name</label>
                <input value={form.customerName} onChange={e => setForm({...form, customerName: e.target.value})} placeholder="Full name" />
              </div>
              <div className="form-group">
                <label>Email</label>
                <input type="email" value={form.customerEmail} onChange={e => setForm({...form, customerEmail: e.target.value})} placeholder="email@example.com" />
              </div>
            </div>

            <div style={{ marginTop: 16 }}>
              <label style={{ fontSize: 12, color: 'var(--muted)', textTransform: 'uppercase', letterSpacing: '0.4px' }}>Items</label>
              {form.items.map((item, i) => (
                <div key={i} className="form-row" style={{ marginTop: 8 }}>
                  <div className="form-group" style={{ flex: 2 }}>
                    <select value={item.productId} onChange={e => updateItem(i, 'productId', e.target.value)}>
                      <option value="">Select product</option>
                      {products.map(p => <option key={p.id} value={p.id}>{p.name} — ${p.price}</option>)}
                    </select>
                  </div>
                  <div className="form-group" style={{ flex: 1 }}>
                    <input type="number" min="1" value={item.quantity} onChange={e => updateItem(i, 'quantity', e.target.value)} placeholder="Qty" />
                  </div>
                  {form.items.length > 1 && (
                    <button className="btn btn-danger btn-sm" onClick={() => removeItem(i)} style={{ alignSelf: 'center' }}>✕</button>
                  )}
                </div>
              ))}
              <button className="btn btn-ghost btn-sm" style={{ marginTop: 8 }} onClick={addItem}>+ Add item</button>
            </div>

            <div className="form-actions">
              <button className="btn btn-ghost" onClick={() => setModal(false)}>Cancel</button>
              <button className="btn btn-primary" onClick={handleCreate}>Place Order</button>
            </div>
          </div>
        </div>
      )}
    </>
  )
}
