import './Package_Details.css'
import '../Packages_List/Packages_List.css'

import { type Package } from '../../types/types'
import { useState } from 'react'
import { useParams  } from 'react-router-dom'
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

const Package_Details = ({ packages, setPackages }: PackagesProps) => {
  const { trackingNumber } = useParams<{ trackingNumber: string }>();
  const trackingNumberInt = parseInt(trackingNumber!);
  const pkg = packages.find(pkg => pkg.trackingNumber === trackingNumberInt)!;


  const [hoveredPkg, setHoveredPkg] = useState<number | null>(null)



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

  if (!pkg) {
    return <div>Loading package...</div>;
  }

  return (
    <div className='pageWrapper'>
        <div className='package'>
          <h1>Package Details</h1>
          <div className='packageWrapper'>
            <div className='packageDetails'>
              <h3>Name</h3>
              <ul>{pkg.name}</ul>
              <div className='statusContainer'>
                <h3>Current status</h3>
                {statusWithMenu(pkg, hoveredPkg, setHoveredPkg, updateStatus)}
                <div>{new Date(
                    pkg.statuses[pkg.statuses.length - 1].timestamp
                  ).toLocaleString()}</div>
              </div>

              <div className='sender'>
                <h3>Sender</h3>
                <ul>{pkg.sender.name}</ul>
                <ul>{pkg.sender.address}</ul>
                <ul>{pkg.sender.phone}</ul>
              </div>
              
              <div className='recipient'>
                <h3>Recipient</h3>
                <ul>{pkg.recipient.name}</ul>
                <ul>{pkg.recipient.address}</ul>
                <ul>{pkg.recipient.phone}</ul>
              </div>
            </div>
            
            <div className='packageTimeline'>
              <h3>Status timeline</h3>
              {packageTimeline(pkg.statuses)}
            </div>
          </div>
        </div>
    </div>
  )
}

const packageTimeline = (statuses: Package['statuses']) => {
  const rows = statuses.map(status => 
    <tr>
      <td>{status.statusName}</td>
      <td>{new Date(status.timestamp).toLocaleString()}</td>
    </tr>
  )

  return (
    <table>
      <tbody>
        {rows}
      </tbody>
    </table>
  )
}

const statusWithMenu = (
  pkg: Package,
  hoveredPkg: number | null,
  setHoveredPkg: (val: number | null) => void,
  updateStatus: (trackingNumber: number, newStatus: string) => void
) => {
  return (
    <div
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
    </div>
  )
}

export default Package_Details