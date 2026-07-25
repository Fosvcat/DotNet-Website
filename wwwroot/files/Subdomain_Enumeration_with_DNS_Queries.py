import dns.resolver
import dns.reversename

def enumerate_subdomains(domain, wordlist):
    subdomains = []
    for sub in wordlist:
        target = f"{sub}.{domain}"
        try:
            answers = dns.resolver.resolve(target, 'A')
            for rdata in answers:
                subdomains.append((target, rdata.address))
        except (dns.resolver.NXDOMAIN, dns.resolver.NoAnswer):
            continue
    return subdomains

# Example usage with a small wordlist
wordlist = ['www', 'mail', 'ftp', 'dev', 'api', 'test']
results = enumerate_subdomains('example.com', wordlist)
for sub, ip in results:
    print(f"{sub} -> {ip}")