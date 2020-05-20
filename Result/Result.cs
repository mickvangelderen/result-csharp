using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

// ReSharper disable InconsistentNaming

namespace Result
{
    [Serializable]
    public class ResultUnwrapException<E> : Exception
    {
        public E Inner { get; }

        public ResultUnwrapException(E inner)
        {
            Inner = inner;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected ResultUnwrapException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Inner = (E)info.GetValue("Inner", typeof(E));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Inner", Inner, typeof(E));
            base.GetObjectData(info, context);
        }
    }
    
    public abstract class Result<T, E>: IEquatable<Result<T, E>>
    {
        public static Result<T, E> Ok(T ok) => new ResultOk(ok);

        public static Result<T, E> Err(E err) => new ResultErr(err);
        
        public static Result<T, E> NotNullOr(T t, E e) => t != null ? Ok(t) : Err(e);

        public static Result<T, E> NotNullOrElse(T t, Func<E> f) => t != null ? Ok(t) : Err(f());

        public abstract Result<U, E> And<U>(Result<U, E> r);

        public abstract Result<U, E> AndThen<U>(Func<T, Result<U, E>> f);

        public abstract Result<T, F> Or<F>(Result<T, F> r);

        public abstract Result<T, F> OrElse<F>(Func<E, Result<T, F>> f);

        public abstract Result<U, E> Map<U>(Func<T, U> f);

        public abstract Result<T, F> MapErr<F>(Func<E, F> f);

        public abstract T Unwrap();

        public abstract T UnwrapOr(T t);

        public abstract T UnwrapOrElse(Func<E, T> f);

        public abstract bool Equals(Result<T, E> other);

        internal sealed class ResultOk: Result<T, E>
        {
            internal T Value { get; }

            internal ResultOk(T value)
            {
                Value = value;
            }

            public override Result<U, E> And<U>(Result<U, E> r) => r;

            public override Result<U, E> AndThen<U>(Func<T, Result<U, E>> f) => f(Value);

            public override Result<T, F> Or<F>(Result<T, F> r) => Result<T, F>.Ok(Value);

            public override Result<T, F> OrElse<F>(Func<E, Result<T, F>> f) => Result<T, F>.Ok(Value);

            public override Result<U, E> Map<U>(Func<T, U> f) => Result<U, E>.Ok(f(Value));

            public override Result<T, F> MapErr<F>(Func<E, F> f) => Result<T, F>.Ok(Value);

            public override T Unwrap() => Value;

            public override T UnwrapOr(T t) => Value;

            public override T UnwrapOrElse(Func<E, T> f) => Value;

            public override bool Equals(Result<T, E> other) => other is ResultOk ok && ok.Value.Equals(Value);

            public override bool Equals(object obj) => obj is ResultOk ok && ok.Value.Equals(Value);

            public override int GetHashCode() => Value.GetHashCode();

            public override string ToString() => $"Value({Value})";
        }

        internal sealed class ResultErr : Result<T, E>
        {
            internal E Value { get; }

            internal ResultErr(E value)
            {
                Value = value;

            }

            public override Result<U, E> And<U>(Result<U, E> r) => Result<U, E>.Err(Value);

            public override Result<U, E> AndThen<U>(Func<T, Result<U, E>> f) => Result<U, E>.Err(Value);

            public override Result<T, F> Or<F>(Result<T, F> r) => r;

            public override Result<T, F> OrElse<F>(Func<E, Result<T, F>> f) => f(Value);

            public override Result<U, E> Map<U>(Func<T, U> f) => Result<U, E>.Err(Value);

            public override Result<T, F> MapErr<F>(Func<E, F> f) => Result<T, F>.Err(f(Value));

            public override T Unwrap() => throw new ResultUnwrapException<E>(Value);

            public override T UnwrapOr(T t) => t;

            public override T UnwrapOrElse(Func<E, T> f) => f(Value);

            public override bool Equals(Result<T, E> other) => other is ResultErr err && err.Value.Equals(Value);
            
            public override bool Equals(object obj) => obj is ResultErr err && err.Value.Equals(Value);

            public override int GetHashCode() => Value.GetHashCode();

            public override string ToString() => $"Err({Value})";
        }
    }
}
