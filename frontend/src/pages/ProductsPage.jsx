import { useState, useEffect } from 'react'
import { getProducts, getCategories, createProduct, updateProduct, deleteProduct } from '../services/api'

const EMPTY_FORM = { name: '', description: '', price: '', stock: '', categoryId: '' }

export default function ProductsPage() {
  const [products, setProducts] = useState([])
  const [categories, setCategories] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [modal, setModal] = useState(null) // null | 'create' | product (for edit)
  const [form, setForm] = useState(EMPTY_FORM)
  const [filterCat, setFilterCat] = useState('')
  const [selectedLinks, setSelectedLinks] = useState(null)

  const load = async () => {
    try {
      setLoading(true)
      const [p, c] = await Promise.all([getProducts(filterCat || null), getCategories()])
      setProducts(p)
      setCategories(c)
      setError('')
    } catch (e) {
      setError(e.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { load() }, [filterCat])

  const openCreate = () => { setForm(EMPTY_FORM); setModal('create') }
  const openEdit = (p) => {
    setForm({ name: p.name, description: p.description || '', price: p.price, stock: p.stock, categoryId: p.categoryId })
    setModal(p)
  }

  const handleSubmit = async () => {
    try {
      const data = { ...form, price: parseFloat(form.price), stock: parseInt(form.stock), categoryId: parseInt(form.categoryId) }
      if (modal === 'create') await createProduct(data)
      else await updateProduct(modal.id, data)
      setModal(null)
      load()
    } catch (e) { setError(e.message) }
  }

  const handleDelete = async (id) => {
    if (!confirm('Delete this product?')) return
    try { await deleteProduct(id); load() } catch (e) { setError(e.message) }
  }

  return (
    <>
      <h1 className="page-title">Products <span>{products.length} items</span></h1>

      {error && <div className="error-msg">{error}</div>}

      <div className="card">
        <div className="card-header">
          <div style={{ display: 'flex', gap: 10, alignItems: 'center' }}>
            <select value={filterCat} onChange={e => setFilterCat(e.target.value)} style={{ width: 180 }}>
              <option value="">All categories</option>
              {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
            </select>
          </div>
          <button className="btn btn-primary" onClick={openCreate}>+ Add Product</button>
        </div>

        {loading ? <div className="loading">Loading…</div> : (
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>ID</th><th>Name</th><th>Category</th>
                  <th>Price</th><th>Stock</th><th>Links</th><th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {products.length === 0 ? (
                  <tr><td colSpan={7} className="empty-state">No products found.</td></tr>
                ) : products.map(p => (
                  <tr key={p.id}>
                    <td style={{ color: 'var(--muted)', fontSize: 12 }}>#{p.id}</td>
                    <td><strong>{p.name}</strong><br/><span style={{ fontSize: 12, color: 'var(--muted)' }}>{p.description}</span></td>
                    <td>{p.categoryName}</td>
                    <td>${p.price.toFixed(2)}</td>
                    <td>{p.stock}</td>
                    <td>
                      <button className="btn btn-ghost btn-sm" onClick={() => setSelectedLinks(p.links)}>🔗 View</button>
                    </td>
                    <td style={{ display: 'flex', gap: 6 }}>
                      <button className="btn btn-teal btn-sm" onClick={() => openEdit(p)}>Edit</button>
                      <button className="btn btn-danger btn-sm" onClick={() => handleDelete(p.id)}>Del</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Create/Edit modal */}
      {modal !== null && (
        <div className="modal-overlay" onClick={() => setModal(null)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <div className="modal-title">{modal === 'create' ? 'New Product' : `Edit: ${modal.name}`}</div>
            <div className="form-grid">
              <div className="form-group full">
                <label>Name</label>
                <input value={form.name} onChange={e => setForm({...form, name: e.target.value})} placeholder="Product name" />
              </div>
              <div className="form-group full">
                <label>Description</label>
                <textarea value={form.description} onChange={e => setForm({...form, description: e.target.value})} placeholder="Optional description" />
              </div>
              <div className="form-group">
                <label>Price ($)</label>
                <input type="number" step="0.01" value={form.price} onChange={e => setForm({...form, price: e.target.value})} placeholder="0.00" />
              </div>
              <div className="form-group">
                <label>Stock</label>
                <input type="number" value={form.stock} onChange={e => setForm({...form, stock: e.target.value})} placeholder="0" />
              </div>
              <div className="form-group full">
                <label>Category</label>
                <select value={form.categoryId} onChange={e => setForm({...form, categoryId: e.target.value})}>
                  <option value="">Select category</option>
                  {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                </select>
              </div>
            </div>
            <div className="form-actions">
              <button className="btn btn-ghost" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary" onClick={handleSubmit}>{modal === 'create' ? 'Create' : 'Save'}</button>
            </div>
          </div>
        </div>
      )}

      {/* HATEOAS links modal */}
      {selectedLinks && (
        <div className="modal-overlay" onClick={() => setSelectedLinks(null)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <div className="modal-title">HATEOAS Links</div>
            <p style={{ color: 'var(--muted)', fontSize: 13, marginBottom: 12 }}>
              These are the hypermedia links embedded in this resource response — this is what makes the API Level 4.
            </p>
            {Object.entries(selectedLinks).map(([rel, url]) => (
              <div key={rel} style={{ marginBottom: 8 }}>
                <label>{rel}</label>
                <input readOnly value={url} style={{ width: '100%', marginTop: 4 }} />
              </div>
            ))}
            <div className="form-actions">
              <button className="btn btn-ghost" onClick={() => setSelectedLinks(null)}>Close</button>
            </div>
          </div>
        </div>
      )}
    </>
  )
}
