using System;

/// <summary>
/// https://qiita.com/keroxp/items/dfcbaf7e743eda3cdbe1
/// セキュアな数値のメモリ上での改ざんを防ぐ
/// </summary>
namespace Secure
{

    public interface ISecureValue<T>
    {
        /// <summary>
        /// 暗号化した値を復号してGet.
        /// 暗号化したいセキュアな値のSet.
        /// </summary>
        T Value { get; set; }
    }

    /// <summary>
    /// XOR演算の特性（数値が反転する）を利用してセキュアな数値をマスクしてメモリ上に保持する初期化Class.
    /// </summary>
    public static class SecureValues
    {
        /// <summary>
        /// Intで初期化.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static ISecureValue<int> Int(int v = 0)
        {
            return new SecureInt(v);
        }
        /// <summary>
        /// UIntで初期化.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static ISecureValue<uint> UInt(uint v = 0)
        {
            return new SecureUInt(v);
        }
        /// <summary>
        /// Floatで初期化.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static ISecureValue<float> Float(float v = 0)
        {
            return new SecureFloat(v);
        }
    }

    [Serializable]
    internal class SecureInt : ISecureValue<int>
    {
        private int _value;
        private readonly int seed;
        public int Value
        {
            get { return _value ^ seed; }
            set { _value = value ^ seed; }
        }

        public SecureInt(int value = 0)
        {
            var rnd = new Random();
            seed = rnd.Next() << 32 | rnd.Next();
            Value = value;
        }
    }

    [Serializable]
    internal class SecureUInt : ISecureValue<uint>
    {
        private uint _value;
        private readonly uint seed;

        public SecureUInt(uint value = 0)
        {
            var rnd = new Random();
            seed = (uint)(rnd.Next() << 32 | rnd.Next());
            Value = value;
        }
        public uint Value
        {
            get { return _value ^ seed; }
            set { _value = value ^ seed; }
        }
    }

    [Serializable]
    internal class SecureFloat : ISecureValue<float>
    {
        private readonly byte[] _bytes;
        private byte[] _buffer;
        private readonly byte seed;
        public SecureFloat(float v = 0)
        {
            _buffer = new byte[4];
            _bytes = new byte[4];
            var rnd = new Random();
            seed = (byte)(rnd.Next() << 32 | rnd.Next());
            Value = v;
        }

        public float Value
        {
            get
            {
                _bytes.CopyTo(_buffer, 0);
                Xor(ref _buffer);
                return BitConverter.ToSingle(_buffer, 0);
            }
            set
            {
                _buffer = BitConverter.GetBytes(value); //BitConverterを利用してbyte配列に変換して保存
                Xor(ref _buffer);
                _buffer.CopyTo(_bytes, 0);
            }
        }

        private void Xor(ref byte[] arr)
        {
            arr[0] ^= seed;
            arr[1] ^= seed;
            arr[2] ^= seed;
            arr[3] ^= seed;
        }
    }
}