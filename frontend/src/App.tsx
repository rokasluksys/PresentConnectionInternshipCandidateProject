import { BrowserRouter, Route, Router, Routes } from "react-router-dom"
import { ToastContainer } from "react-toastify"
import { useState, useEffect } from "react"

import Packages_List from "./pages/packages_list/Packages_List"
import { type Package } from './types/types'
import Package_Create from "./pages/package_create/Package_Create"
import API_requests from './services/API_requests'
import Package_Details from "./pages/package_details/Package_Details"

const App = () => {
  const [packages, setPackages] = useState<Package[]>([])

  useEffect(() => {
      const fetchPackages = async () => {
        try {
          const fetchedOrders = await API_requests.getAll()
          setPackages(fetchedOrders)
        } catch (error) {
          console.error('Error fetching orders:', error)
        }
      }

      fetchPackages()
    }, [])

  return (
    <BrowserRouter>
      <ToastContainer
        position="top-right"
        autoClose={3000}
        hideProgressBar={false}
        newestOnTop={false}
        closeOnClick
        pauseOnFocusLoss={false}
        draggable
        pauseOnHover
        theme="light"
      />
      <Routes>
          <Route path="/" element={<Packages_List packages={packages} setPackages={setPackages}/>} />
          <Route path="/package_create" element={<Package_Create packages={packages} setPackages={setPackages}/>} />
          <Route path="/package_details/:trackingNumber" element={<Package_Details packages={packages} setPackages={setPackages}/>} />
      </Routes>
    </BrowserRouter>
  )
}

export default App