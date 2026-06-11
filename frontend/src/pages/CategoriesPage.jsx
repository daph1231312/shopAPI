import { useState, useEffect } from 'react'
import { getCategories, createCategory, deleteCategory } from '../services/api'

export default function CategoriesPage() {
  const [categories, setCategories] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [form, setForm] = useState({ name: '', description: '' })
  const [creating, setCreating] = useState(false)

  const load = async () => {
    try {
      setLoading(true)
      setCategories(await getCategories())
      setError('')
    } catch (e) { setError(e.message) }
    finally { setLoading(false) }
  }

  useEffect(() => { load() }, [])

  const handleCreate = async () => {
    try {
      await createCategory(form)
      setForm({ name: '', description: '' })
      setCreating(false)
      load()
    } catch (e) { setError(e.message) }
  }

  const handleDelete = async (id) => {
    if (!confirm('Delete this category?')) return
    try { await deleteCategory(id); load() } catch (e) { setError(e.message) }
  }

  return (
    <>
      <h1 className="page-title">Categories <span>{categories.length} total</span></h1>
      {error && <div className="error-msg">{error}</div>}

      <div className="card">
        <div className="card-header">
          <span className="card-title">All Categories</span>
          <button className="btn btn-primary" onClick={() => setCreating(v => !v)}>
            {creating ? 'Cancel' : '+ New Category'}
          </button>
        </div>

        {creating && (
          <div style={{ marginBottom: 20, padding: '16px', background: 'var(--surface2)', borderRadius: 8 }}>
            <div className="form-grid">
              <div className="form-group">
                <label>Name</label>
                <input value={form.name} onChange={e => setForm({...form, name: e.target.value})} placeholder="Category name" />
              </div>
              <div className="form-group">
                <label>Description</label>
                <input value={form.description} onChange={e => setForm({...form, description: e.target.value})} placeholder="Optional" />
              </div>
            </div>
            <div className="form-actions">
              <button className="btn btn-primary" onClick={handleCreate}>Create</button>
            </div>
          </div>
        )}

        {loading ? <div className="loading">Loading…</div> : (
          <div className="table-wrap">
            <table>
              <thead>
                <tr><th>ID</th><th>Name</th><th>Description</th><th>Products</th><th>Actions</th></tr>
              </thead>
              <tbody>
                {categories.length === 0 ? (
                  <tr><td colSpan={5} className="empty-state">No categories.</td></tr>
                ) : categories.map(c => (
                  <tr key={c.id}>
                    <td style={{ color: 'var(--muted)', fontSize: 12 }}>#{c.id}</td>
                    <td><strong>{c.name}</strong></td>
                    <td style={{ color: 'var(--muted)' }}>{c.description || '—'}</td>
                    <td>{c.productCount}</td>
                    <td>
                      <button className="btn btn-danger btn-sm" onClick={() => handleDelete(c.id)}>Delete</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </>
  )
}
