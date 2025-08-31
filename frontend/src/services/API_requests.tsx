import axios from "axios"
import type { PackageDTO } from "../types/types"

const baseUrl : string = 'https://localhost:7061/api/packages'

const getAll = async () => {
  const response = await axios.get(baseUrl)
  return response.data
}

const updateStatus = async (trackingNumber: number, newStatus: string) => {
  const url = `${baseUrl}/${trackingNumber}`

  const response = await axios.patch(
    url,
    { newStatus }
  )
  return response.data
}

const createPackage = async (newPackage: PackageDTO) => {
  const response = await axios.post(baseUrl, newPackage)
  return response.data
}

export default {
  getAll,
  updateStatus,
  createPackage
}