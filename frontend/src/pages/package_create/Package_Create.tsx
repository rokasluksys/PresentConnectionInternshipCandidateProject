import './Package_Create.css'
import { type Package, type Contact, type PackageDTO } from '../../types/types'
import { useEffect, useState } from 'react'
import { toast } from 'react-toastify'
import { useNavigate } from 'react-router-dom'
import API_requests from '../../services/API_requests'

interface PackagesProps {
  packages: Package[]
  setPackages: React.Dispatch<React.SetStateAction<Package[]>>
}

const Package_Create = ({ packages, setPackages }: PackagesProps) => {
  const [packageName, setPackageName] = useState('')

  const [senderName, setSenderName] = useState('')
  const [senderAddress, setSenderAddress] = useState('')
  const [senderPhone, setSenderPhone] = useState('')
  const [senderSuggestions, setSenderSuggestions] = useState<Contact[]>([])
  const [senderFocused, setSenderFocused] = useState(false);

  const [recipientName, setRecipientName] = useState('')
  const [recipientAddress, setRecipientAddress] = useState('')
  const [recipientPhone, setRecipientPhone] = useState('')
  const [recipientSuggestions, setRecipientSuggestions] = useState<Contact[]>([])
  const [recipientFocused, setRecipientFocused] = useState(false);

  const [allContacts, setAllContacts] = useState<Contact[]>([])

  const navigate = useNavigate()

  //all contacts array to be used for autofill suggestions
  useEffect(() => {
    const contacts = Array.from(
      new Map(
        packages
          .flatMap(p => [p.sender, p.recipient])
          .map(c => [c.contactID, c])
      ).values()
    )

    contacts.sort((a, b) => {
      const nameA = a.name.toUpperCase();
      const nameB = b.name.toUpperCase();
      if (nameA < nameB) {
        return -1;
      }
      if (nameA > nameB) {
        return 1;
      }
      return 0;
    });

    setAllContacts(contacts)
  }, [packages])

  //autofill suggestions on sender name
  useEffect(() => {
    if (!senderFocused) {
      setSenderSuggestions([]);
      return;
    }

    const filtered = allContacts.filter(c =>
      c.name.toLowerCase().includes(senderName.toLowerCase())
    );

    setSenderSuggestions(filtered);
  }, [senderName, senderFocused, allContacts]);

  //autofill suggestions on recipient name
  useEffect(() => {
    if (!recipientFocused) {
      setRecipientSuggestions([]);
      return;
    }

    const filtered = allContacts.filter(c =>
      c.name.toLowerCase().includes(recipientName.toLowerCase())
    );

    setRecipientSuggestions(filtered);
  }, [recipientName, recipientFocused, allContacts]);

  const createPackage = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    const newPackage: PackageDTO = {
      name: packageName,
      sender: {
        name: senderName,
        address: senderAddress,
        phone: senderPhone
      },
      recipient: {
        name: recipientName,
        address: recipientAddress,
        phone: recipientPhone
      }
    }

    try {
      const response = await API_requests.createPackage(newPackage)
      toast.success('Package created!')
      setPackages(prev => [...prev, response])
      navigate('/')
    } catch (error) {
      toast.error("ERROR")
    }
  }

  const createPackageForm = () => (
    <form className='form' onSubmit={createPackage}>
      <h3>Package name</h3>
      <div>
        <input
          type='text'
          value={packageName}
          name='createdPackageName'
          onChange={({ target }) => setPackageName(target.value)}
        />
      </div>

      <div className='sender'>
        <h3>Sender</h3>
        <div style={{ position: 'relative' }}>
          <input
            type='text'
            value={senderName}
            onFocus={() => setSenderFocused(true)}
            onBlur={() => setSenderFocused(false)}
            onChange={({ target }) => {setSenderName(target.value)}}
            placeholder='Name - new or choose from existing'
          />
          {senderSuggestions.length > 0 && (
            <ul className='suggestions'>
              {senderSuggestions.map(c => (
                <li
                  key={c.contactID}
                  onMouseDown={() => {
                    setSenderName(c.name)
                    setSenderAddress(c.address)
                    setSenderPhone(c.phone)
                    setSenderSuggestions([])
                  }}
                >
                  {c.name} ({c.phone})
                </li>
              ))}
            </ul>
          )}
        </div>
        <input
          type='text'
          value={senderAddress}
          onChange={({ target }) => setSenderAddress(target.value)}
          placeholder='Address'
        />
        <input
          type='text'
          value={senderPhone}
          onChange={({ target }) => setSenderPhone(target.value)}
          placeholder='Phone #'
        />
      </div>

      <div className='recipient'>
        <h3>Recipient</h3>
        <div style={{ position: 'relative' }}>
          <input
            type='text'
            value={recipientName}
            onFocus={() => setRecipientFocused(true)}
            onBlur={() => setRecipientFocused(false)}
            onChange={({ target }) => {setRecipientName(target.value)}}
            placeholder='Name - new or choose from existing'
          />
          {recipientSuggestions.length > 0 && (
            <ul className='suggestions'>
              {recipientSuggestions.map(c => (
                <li
                  key={c.contactID}
                  onMouseDown={() => {
                    setRecipientName(c.name)
                    setRecipientAddress(c.address)
                    setRecipientPhone(c.phone)
                    setRecipientSuggestions([])
                  }}
                >
                  {c.name} ({c.phone})
                </li>
              ))}
            </ul>
          )}
        </div>
        <input
          type='text'
          value={recipientAddress}
          onChange={({ target }) => setRecipientAddress(target.value)}
          placeholder='Address'
        />
        <input
          type='text'
          value={recipientPhone}
          onChange={({ target }) => setRecipientPhone(target.value)}
          placeholder='Phone #'
        />
      </div>

      <button type='submit'>Create Package</button>
    </form>
  )

  return (
    <div className="pageWrapper">
      <title>Create Package</title>
      <div className='createPackageContainer'>
        <h1>Create Package</h1>
        {createPackageForm()}
      </div>
    </div>
  )
}

export default Package_Create