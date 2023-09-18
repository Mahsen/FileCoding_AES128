﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileCoding
{
    class AES
    {
        const int Nb = 4;
        const int Nk = 4;
        const int KeyLen = 16;
        const int Nr = 10;
        const int MULTIPLY_AS_A_FUNCTION = 0;


        private byte[][] state = new byte[4][] {
        new byte[4]{0,0,0,0},
        new byte[4]{0,0,0,0},
        new byte[4]{0,0,0,0},
        new byte[4]{0,0,0,0}
        };
        private byte[] RoundKey = new byte[176];

        private byte[] Key = new byte[KeyLen];
        //public void SetKey(byte[] key)
        //{
        //    for (int i = 0; i < KeyLen; i++)
        //    {
        //        this.Key[i] = key[i];
        //    }

        //}

        private byte[] IV = new byte[KeyLen];
        //public void SetIV(byte[] iv)
        //{
        //    for (int i = 0; i < KeyLen; i++)
        //    {
        //        this.IV[i] = iv[i];
        //    }
        //}

        byte[] sbox = new byte[256] {
            //0     1    2      3     4    5     6     7      8    9     A      B    C     D     E     F
            0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76,
            0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0,
            0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15,
            0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75,
            0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84,
            0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf,
            0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8,
            0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2,
            0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73,
            0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb,
            0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79,
            0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08,
            0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a,
            0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e,
            0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf,
            0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16 
        };
        private byte GetSbox(int index)
        {
            return this.sbox[index];
        }


        private byte[] rsbox = new byte[256]{ 
            0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb,
            0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87, 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb,
            0x54, 0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c, 0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e,
            0x08, 0x2e, 0xa1, 0x66, 0x28, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25,
            0x72, 0xf8, 0xf6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92,
            0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9, 0xda, 0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84,
            0x90, 0xd8, 0xab, 0x00, 0x8c, 0xbc, 0xd3, 0x0a, 0xf7, 0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06,
            0xd0, 0x2c, 0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02, 0xc1, 0xaf, 0xbd, 0x03, 0x01, 0x13, 0x8a, 0x6b,
            0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 0xdc, 0xea, 0x97, 0xf2, 0xcf, 0xce, 0xf0, 0xb4, 0xe6, 0x73,
            0x96, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 0x6e,
            0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89, 0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b,
            0xfc, 0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20, 0x9a, 0xdb, 0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4,
            0x1f, 0xdd, 0xa8, 0x33, 0x88, 0x07, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f,
            0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 0x0d, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef,
            0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0, 0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61,
            0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d
        };
        private byte GetRSbox(int index)
        {
            return this.rsbox[index];
        }
        byte[] Rcon = new byte[255]{
                0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 
                0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 
                0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 
                0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 
                0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 
                0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 
                0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 
                0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 
                0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 
                0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 
                0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6, 0x97, 0x35, 
                0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 0x61, 0xc2, 0x9f, 
                0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb, 0x8d, 0x01, 0x02, 0x04, 
                0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36, 0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 
                0xc6, 0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91, 0x39, 0x72, 0xe4, 0xd3, 0xbd, 
                0x61, 0xc2, 0x9f, 0x25, 0x4a, 0x94, 0x33, 0x66, 0xcc, 0x83, 0x1d, 0x3a, 0x74, 0xe8, 0xcb
        };



        private void KeyExpantion()
        {
            UInt32 i, j, k;
            byte[] tempa = new byte[4]; // Used for the column/row operations

            // The first round key is the key itself.
            for (i = 0; i < Nk; ++i)
            {
                RoundKey[(i * 4) + 0] = this.Key[(i * 4) + 0];
                RoundKey[(i * 4) + 1] = this.Key[(i * 4) + 1];
                RoundKey[(i * 4) + 2] = this.Key[(i * 4) + 2];
                RoundKey[(i * 4) + 3] = this.Key[(i * 4) + 3];
            }

            // All other round keys are found from the previous round keys.
            for (; (i < (Nb * (Nr + 1))); ++i)
            {
                for (j = 0; j < 4; ++j)
                {
                    tempa[j] = RoundKey[(i - 1) * 4 + j];
                }
                if (i % Nk == 0)
                {
                    // This function rotates the 4 bytes in a word to the left once.
                    // [a0,a1,a2,a3] becomes [a1,a2,a3,a0]

                    // Function RotWord()
                    {
                        k = tempa[0];
                        tempa[0] = tempa[1];
                        tempa[1] = tempa[2];
                        tempa[2] = tempa[3];
                        tempa[3] = (byte)k;
                    }

                    // SubWord() is a function that takes a four-byte input word and 
                    // applies the S-box to each of the four bytes to produce an output word.

                    // Function Subword()
                    {
                        tempa[0] = this.GetSbox(tempa[0]);
                        tempa[1] = this.GetSbox(tempa[1]);
                        tempa[2] = this.GetSbox(tempa[2]);
                        tempa[3] = this.GetSbox(tempa[3]);
                    }

                    tempa[0] = (byte)(tempa[0] ^ Rcon[i / Nk]);
                }
                else if (Nk > 6 && i % Nk == 4)
                {
                    // Function Subword()
                    {
                        tempa[0] = this.GetSbox(tempa[0]);
                        tempa[1] = this.GetSbox(tempa[1]);
                        tempa[2] = this.GetSbox(tempa[2]);
                        tempa[3] = this.GetSbox(tempa[3]);
                    }
                }
                RoundKey[i * 4 + 0] = (byte)(RoundKey[(i - Nk) * 4 + 0] ^ tempa[0]);
                RoundKey[i * 4 + 1] = (byte)(RoundKey[(i - Nk) * 4 + 1] ^ tempa[1]);
                RoundKey[i * 4 + 2] = (byte)(RoundKey[(i - Nk) * 4 + 2] ^ tempa[2]);
                RoundKey[i * 4 + 3] = (byte)(RoundKey[(i - Nk) * 4 + 3] ^ tempa[3]);
            }
        }

        private void AddRoundKey(byte round)
        {
            byte i, j;
            for (i = 0; i < 4; ++i)
            {
                for (j = 0; j < 4; ++j)
                {
                    state[i][j] ^= RoundKey[round * Nb * 4 + i * Nb + j];
                }
            }
        }

        private void SubBytes()
        {
            byte i, j;
            for (i = 0; i < 4; ++i)
            {
                for (j = 0; j < 4; ++j)
                {
                    state[j][i] = this.GetSbox(state[j][i]);
                }
            }
        }

        private void ShiftRows()
        {
            byte temp;

            // Rotate first row 1 columns to left  
            temp = state[0][1];
            state[0][1] = state[1][1];
            state[1][1] = state[2][1];
            state[2][1] = state[3][1];
            state[3][1] = temp;

            // Rotate second row 2 columns to left  
            temp = state[0][2];
            state[0][2] = state[2][2];
            state[2][2] = temp;

            temp = state[1][2];
            state[1][2] = state[3][2];
            state[3][2] = temp;

            // Rotate third row 3 columns to left
            temp = state[0][3];
            state[0][3] = state[3][3];
            state[3][3] = state[2][3];
            state[2][3] = state[1][3];
            state[1][3] = temp;
        }

        private byte xtime(byte x)
        {
            return (byte)((x << 1) ^ (((x >> 7) & 1) * 0x1b));
        }

        private void MixColumns()
        {
            byte i;
            byte Tmp, Tm, t;
            for (i = 0; i < 4; ++i)
            {
                t = state[i][0];
                Tmp = (byte)(state[i][0] ^ state[i][1] ^ state[i][2] ^ state[i][3]);
                Tm = (byte)(state[i][0] ^ state[i][1]); Tm = xtime(Tm); state[i][0] ^= (byte)(Tm ^ Tmp);
                Tm = (byte)(state[i][1] ^ state[i][2]); Tm = xtime(Tm); state[i][1] ^= (byte)(Tm ^ Tmp);
                Tm = (byte)(state[i][2] ^ state[i][3]); Tm = xtime(Tm); state[i][2] ^= (byte)(Tm ^ Tmp);
                Tm = (byte)(state[i][3] ^ t); Tm = xtime(Tm); state[i][3] ^= (byte)(Tm ^ Tmp);
            }
        }

        private byte Multiply(byte x, byte y)
        {
            return (byte)(((y & 1) * x) ^
                 ((y >> 1 & 1) * xtime(x)) ^
                 ((y >> 2 & 1) * xtime(xtime(x))) ^
                 ((y >> 3 & 1) * xtime(xtime(xtime(x)))) ^
                 ((y >> 4 & 1) * xtime(xtime(xtime(xtime(x))))));
        }

        private void InvMixColumns()
        {
            int i;
            byte a, b, c, d;
            for (i = 0; i < 4; ++i)
            {
                a = state[i][0];
                b = state[i][1];
                c = state[i][2];
                d = state[i][3];

                state[i][0] = (byte)(Multiply(a, 0x0e) ^ Multiply(b, 0x0b) ^ Multiply(c, 0x0d) ^ Multiply(d, 0x09));
                state[i][1] = (byte)(Multiply(a, 0x09) ^ Multiply(b, 0x0e) ^ Multiply(c, 0x0b) ^ Multiply(d, 0x0d));
                state[i][2] = (byte)(Multiply(a, 0x0d) ^ Multiply(b, 0x09) ^ Multiply(c, 0x0e) ^ Multiply(d, 0x0b));
                state[i][3] = (byte)(Multiply(a, 0x0b) ^ Multiply(b, 0x0d) ^ Multiply(c, 0x09) ^ Multiply(d, 0x0e));
            }
        }
        private void InvSubBytes()
        {
            byte i, j;
            for (i = 0; i < 4; ++i)
            {
                for (j = 0; j < 4; ++j)
                {
                    state[j][i] = this.GetRSbox(state[j][i]);
                }
            }
        }
        private void InvShiftRows()
        {
            byte temp;

            // Rotate first row 1 columns to right  
            temp = state[3][1];
            state[3][1] = state[2][1];
            state[2][1] = state[1][1];
            state[1][1] = state[0][1];
            state[0][1] = temp;

            // Rotate second row 2 columns to right 
            temp = state[0][2];
            state[0][2] = state[2][2];
            state[2][2] = temp;

            temp = state[1][2];
            state[1][2] = state[3][2];
            state[3][2] = temp;

            // Rotate third row 3 columns to right
            temp = state[0][3];
            state[0][3] = state[1][3];
            state[1][3] = state[2][3];
            state[2][3] = state[3][3];
            state[3][3] = temp;
        }

        private void Cipher()
        {
            byte round = 0;

            // Add the First round key to the state before starting the rounds.
            AddRoundKey(0);

            // There will be Nr rounds.
            // The first Nr-1 rounds are identical.
            // These Nr-1 rounds are executed in the loop below.
            for (round = 1; round < Nr; ++round)
            {
                SubBytes();
                ShiftRows();
                MixColumns();
                AddRoundKey(round);
            }

            // The last round is given below.
            // The MixColumns function is not here in the last round.
            SubBytes();
            ShiftRows();
            AddRoundKey(Nr);
        }

        private void InvCipher()
        {
            byte round = 0;

            // Add the First round key to the state before starting the rounds.
            AddRoundKey(Nr);

            // There will be Nr rounds.
            // The first Nr-1 rounds are identical.
            // These Nr-1 rounds are executed in the loop below.
            for (round = Nr - 1; round > 0; round--)
            {
                InvShiftRows();
                InvSubBytes();
                AddRoundKey(round);
                InvMixColumns();
            }

            // The last round is given below.
            // The MixColumns function is not here in the last round.
            InvShiftRows();
            InvSubBytes();
            AddRoundKey(0);
        }

        private void BlockCopy(byte[] output, byte[] input)
        {
            byte i;
            for (i = 0; i < KeyLen; ++i)
            {
                output[i] = input[i];
            }
        }
        
        public void AES128_ECB_encrypt(byte[] input, byte[] key, byte[] output)
        {
            // Copy input to output, and work in-memory on output
            BlockCopy(output, input);

            for (int i = 0; i < 16; i++)
            { 
                state[i/4][i%4] = output[i];
            }

                this.Key = key;
            this.KeyExpantion();
            // The next function call encrypts the PlainText with the Key using AES algorithm.
            Cipher();

            for (int i = 0; i < 16; i++)
            {
                output[i] = state[i / 4][i % 4];
            }
        }
        public void AES128_ECB_decrypt(byte[] input, byte[] key, byte[] output)
        {
            // Copy input to output, and work in-memory on output
            BlockCopy(output, input);
            for (int i = 0; i < 16; i++)
            { 
                state[i/4][i%4] = output[i];
            }

            // The KeyExpansion routine must be called before encryption.
            this.Key = key;
            this.KeyExpantion();

            InvCipher();

            for (int i = 0; i < 16; i++)
            {
                output[i] = state[i / 4][i % 4];
            }
        }

        private void XorWithIv(byte[] buf)
        {
            byte i;
            for (i = 0; i < KeyLen; ++i)
            {
                buf[i] ^= this.IV[i];
            }
        }
        public void AES128_CBC_encrypt_buffer(byte[] output, byte[] input, UInt32 length, byte[] key, byte[] iv)
        {
            byte i;
            byte remainders = (byte)(length % KeyLen); /* Remaining bytes in the last non-full block */

            BlockCopy(output, input);

            //for (i = 0; i < 16; i++)
            //{
            //    state[i / 4][i % 4] = output[i];
            //}

            // Skip the key expansion if key is passed as 0
            if (key != null)
            {
                this.Key = key;
                //KeyExpansion();
                this.KeyExpantion();
            }

            if (iv != null)
            {
                this.IV = iv;
            }

            byte[] tmpOut = new byte[16];
            byte[] tmpIn = new byte[16];

            for (i = 0; i < length; i += KeyLen)
            {
                for (int j = 0; j < 16; j++)
                {
                    tmpIn[j] = input[j + i];
                    tmpOut[j] = output[j + i];
                }
                XorWithIv(tmpIn);
                BlockCopy(tmpOut, tmpIn);
                //state = (state_t*)output;
                for (int j = 0; j < 16; j++)
                {
                    state[j / 4][j % 4] = tmpOut[j];
                }

                Cipher();

                for (int j = 0; j < 16; i++)
                {
                    tmpOut[j] = state[j / 4][j % 4];
                }

                this.IV = tmpOut;

                for (int j = 0; j < 16; j++)
                {
                    output[j + i] = tmpOut[j];
                }

                //input += KEYLEN;
                //output += KEYLEN;
            }

            if (remainders != 0)
            {
                //BlockCopy(output, input);
                //memset(output + remainders, 0, KEYLEN - remainders); /* add 0-padding */
                //state = (state_t*)output;
                //Cipher();
            }
        }
        public void AES128_CBC_decrypt_buffer(byte[] output, byte[] input, byte[] length, byte[] key, byte[] iv)
        {
        }

    }
}
