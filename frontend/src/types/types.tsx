type Package = {
    trackingNumber: number,
    name: string,
    sender: Contact,
    recipient: Contact,
    statuses: Status[]
}

type Contact = {
    contactID: number,
    name: string,
    address: string,
    phone: string
}

type Status = {
    statusName: string,
    timestamp: Date
}

type PackageDTO = {
    name: string,
    sender: ContactDTO,
    recipient: ContactDTO
}

type ContactDTO = {
    name: string,
    address: string,
    phone: string
}

export type { Package, PackageDTO, Contact, Status }