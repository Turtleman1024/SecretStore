# SecretStore API

The **SecretStore** API is a RESTful service that securely stores password entry information, including:

- Website
- Username
- Password

The API encrypts the password using a specified encryption key, ensuring that sensitive data is securely stored.

## Features

- **Store Password Entries**: Securely store website, username, and encrypted password entries.
- **Retrieve Password Entries**: Access password entries, with passwords stored securely.
- **Encryption**: Passwords are encrypted before being stored, using AES encryption.

## Security
- Passwords are encrypted using AES encryption using a user specified EncryptionKey.
- The EncryptionKey is loaded using the Login Controller endpoint [https://localhost:44320/v1/load-key](https://localhost:44320/v1/load-key).


## Notes
- This api is still a work in progress
