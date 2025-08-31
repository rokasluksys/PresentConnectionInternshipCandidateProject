import './Packages_List.css'
import { type Package } from '../../types/types'
import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import API_requests from '../../services/API_requests'
import { toast } from 'react-toastify'

const statusOptionsMap: Record<string, string[]> = {
  Created: ["Sent", "Canceled"],
  Sent: ["Accepted", "Returned", "Canceled"],
  Returned: ["Sent", "Canceled"],
  Accepted: [],
  Canceled: []
}

interface PackagesProps {
  packages: Package[]
  setPackages: React.Dispatch<React.SetStateAction<Package[]>>
}

const Packages = ({ packages, setPackages }: PackagesProps) => {
  const [hoveredPkg, setHoveredPkg] = useState<number | null>(null)
  const [trackingNumberFilter, setTrackingNumberFilter] = useState('')
  const [statusFilter, setStatusFilter] = useState('')

  const columnHeaders: string[] = [
    "Package",
    "Status",
    "Tracking number",
    "Sender",
    "Recipient",
    "Creation Date",
    "Details"
  ]

  const updateStatus = async (trackingNumber: number, newStatus: string) => {
    if (window.confirm("Do you want to change the status?")) {
      try {
        const request = await API_requests.updateStatus(trackingNumber, newStatus)
        toast.success("Status updated!")
        setPackages(prev =>
          prev.map(pkg =>
            pkg.trackingNumber === trackingNumber ? request : pkg
          )
        )
        setHoveredPkg(null)
      } catch (error) {
        toast.error("ERROR")
      }
    }
  }

  return (
    <div className='wrapper'>
      <title>Package List</title>
      {parsePackagesToTable(
        columnHeaders,
        packages,
        hoveredPkg,
        setHoveredPkg,
        updateStatus,
        trackingNumberFilter,
        setTrackingNumberFilter,
        statusFilter,
        setStatusFilter
      )}
      <Link to="/package_create">Create Package</Link>
      {/* <button onClick={() => navigate("/package_create")}>Create Package</button> */}
    </div>
  )
}

const parsePackagesToTable = (
  columnHeaders: string[],
  packages: Package[],
  hoveredPkg: number | null,
  setHoveredPkg: (val: number | null) => void,
  updateStatus: (trackingNumber: number, newStatus: string) => void,
  trackingNumberFilter: string,
  setTrackingNumberFilter: (val: string) => void,
  statusFilter: string,
  setStatusFilter: (val: string) => void
) => {
  const navigate = useNavigate()

  return (
    <div className='wrapper'>
      <table className="ordersTable">
        <thead>
          <tr>
            {columnHeaders.map(header =>
              renderHeaderCell(
                header,
                hoveredPkg,
                setHoveredPkg,
                trackingNumberFilter,
                setTrackingNumberFilter,
                statusFilter,
                setStatusFilter
              )
            )}
          </tr>
        </thead>
        <tbody>
          {packages
            .filter(pkg => 
              trackingNumberFilterMatches(pkg, trackingNumberFilter) &&
              statusFilterMatches(pkg, statusFilter)
            )
            .map(pkg => (
              <tr key={pkg.trackingNumber}>
                <td>{pkg.name}</td>
                {statusWithMenu(pkg, hoveredPkg, setHoveredPkg, updateStatus)}
                <td>{pkg.trackingNumber}</td>
                <td>{pkg.sender.name}</td>
                <td>{pkg.recipient.name}</td>
                <td>
                  {new Date(
                    pkg.statuses[pkg.statuses.length - 1].timestamp
                  ).toLocaleString()}
                </td>
                <td className='details' onClick={() => navigate(`/package_details/${pkg.trackingNumber}`)}>🔍</td>
              </tr>
            ))}
        </tbody>
      </table>
    </div>
  )
}

const renderHeaderCell = (
  header: string,
  hoveredPkg: number | null,
  setHoveredPkg: (val: number | null) => void,
  trackingNumberFilter: string,
  setTrackingNumberFilter: (val: string) => void,
  statusFilter: string,
  setStatusFilter: (val: string) => void
) => {
  if (header === "Tracking number") {
    return headerWithFilter(
      hoveredPkg,
      setHoveredPkg,
      trackingNumberFilter,
      renderTrackingInput,
      setTrackingNumberFilter,
      "Tracking number",
      -1
    )
  }

  if (header === "Status") {
    return headerWithFilter(
      hoveredPkg,
      setHoveredPkg,
      statusFilter,
      renderStatusDropdown,
      setStatusFilter,
      "Status",
      -2
    )
  }

  return <th key={header}>{header}</th>
}

const trackingNumberFilterMatches = (pkg: Package, trackingNumberFilter: string) => {
  if (trackingNumberFilter === '') return true
  if (pkg.trackingNumber.toString().includes(trackingNumberFilter)) {
    return true
  }
  return false
}

const statusFilterMatches = (pkg: Package, statusFilter: string) => {
  if (statusFilter === '') return true
  const currentStatus = pkg.statuses[pkg.statuses.length - 1].statusName
  return currentStatus === statusFilter
}

const headerWithFilter = (
  hoveredPkg: number | null,
  setHoveredPkg: (val: number | null) => void,
  filter: string,
  renderControl: (
    filter: string,
    setFilter: (val: string) => void
  ) => React.ReactNode,
  setFilter: (val: string) => void,
  header: string,
  hoverKey: number
) => {
  return (
    <th
      className="header-cell"
      key={header}
      onMouseEnter={() => setHoveredPkg(hoverKey)}
      onMouseLeave={() => setHoveredPkg(null)}
    >
      {header}
      {(hoveredPkg === hoverKey || filter !== '') && (
        <div className="header-menu">
          <div className="header-menu-title">Filter</div>
          {renderControl(filter, setFilter)}
        </div>
      )}
    </th>
  )
}

const renderTrackingInput = (
  filter: string,
  setFilter: (val: string) => void
) => (
  <input
    type="text"
    value={filter}
    onChange={({ target }) => setFilter(target.value)}
  />
)

const renderStatusDropdown = (
  filter: string,
  setFilter: (val: string) => void
) => (
  <select value={filter} onChange={({ target }) => setFilter(target.value)}>
    <option value="">All</option>
    <option value="Created">Created</option>
    <option value="Sent">Sent</option>
    <option value="Returned">Returned</option>
    <option value="Accepted">Accepted</option>
    <option value="Canceled">Canceled</option>
  </select>
)

const statusWithMenu = (
  pkg: Package,
  hoveredPkg: number | null,
  setHoveredPkg: (val: number | null) => void,
  updateStatus: (trackingNumber: number, newStatus: string) => void
) => {
  return (
    <td
      className="status-cell"
      onMouseEnter={() => setHoveredPkg(pkg.trackingNumber)}
      onMouseLeave={() => setHoveredPkg(null)}
    >
      {pkg.statuses[pkg.statuses.length - 1].statusName}
      {hoveredPkg === pkg.trackingNumber && (
        <div className="status-menu">
          {statusOptionsMap[
            pkg.statuses[pkg.statuses.length - 1].statusName
          ].map(option => (
            <div
              key={option}
              className="status-menu-item"
              onClick={() => updateStatus(pkg.trackingNumber, option)}
            >
              {option}
            </div>
          ))}
        </div>
      )}
    </td>
  )
}

export default Packages
