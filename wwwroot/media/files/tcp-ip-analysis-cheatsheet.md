# TCP/IP Analysis Cheat Sheet

## Application

- DNS resolves names.
- HTTP carries web requests and responses.
- TLS authenticates endpoints and protects application traffic.

## Transport

- TCP handshake: `SYN -> SYN-ACK -> ACK`
- A reset often indicates a reachable host with a closed or rejected port.
- Retransmissions suggest loss, filtering or an unavailable peer.

## Internet

- Confirm source and destination addresses.
- Check route selection and next-hop behavior.

## Link

- Confirm local addressing, ARP and the active interface.

## Troubleshooting order

1. Resolve the expected address.
2. Verify reachability and routing.
3. Inspect transport establishment.
4. Validate TLS and the application response.
