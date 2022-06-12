from base64 import b64encode
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad
from Crypto.Protocol.KDF import PBKDF2
from Crypto.Random import get_random_bytes

def encrypt_dice(key : str, dice: list[int]) -> str:
    data = ' '.join(str(x) for x in dice)
    data = bytes(data, encoding='utf-8')
    salt = get_random_bytes(16)
    salted_key = PBKDF2(key, salt, 32, count=10000)

    cipher = AES.new(salted_key, AES.MODE_CBC)
    ct_bytes = cipher.encrypt(pad(data, AES.block_size))

    result = bytearray(salt)
    result.extend(cipher.iv)
    result.extend(ct_bytes)
    
    return b64encode(result).decode('utf-8')