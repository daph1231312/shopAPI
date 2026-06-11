import { useState } from 'react'
import ProductsPage from './pages/ProductsPage'
import CategoriesPage from './pages/CategoriesPage'
import OrdersPage from './pages/OrdersPage'

const NAV = [
  { id: 'products', label: '📦 Products' },
  { id: 'categories', label: '🗂 Categories' },
  { id: 'orders', label: '🛒 Orders' },
]

export default function App() {
  const [page, setPage] = useState('products')

  return (
    <div className="layout">
      <aside className="sidebar">
        <div className="sidebar-logo">Shop<span>API</span></div>
        {NAV.map(n => (
          <div
            key={n.id}
            className={`nav-item ${page === n.id ? 'active' : ''}`}
            onClick={() => setPage(n.id)}
          >
            {n.label}
          </div>
        ))}
      </aside>
      <main className="main">
        {page === 'products' && <ProductsPage />}
        {page === 'categories' && <CategoriesPage />}
        {page === 'orders' && <OrdersPage />}
      </main>
    </div>
  )
}
