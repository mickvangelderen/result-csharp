using System;
using Xunit;
using FluentAssertions;

namespace Result.Tests
{
    using Result00 = Result<T0, E0>;
    using Result01 = Result<T0, E1>;
    using Result10 = Result<T1, E0>;

    internal class T0: IEquatable<T0>
    {
        internal int Value { get; }

        internal T0(int value)
        {
            Value = value;
        }

        public override string ToString() => $"{nameof(T0)}({Value})";

        public bool Equals(T0 other) => other != null && Value == other.Value;

        public override bool Equals(object obj) => obj is T0 other && Value == other.Value;

        public override int GetHashCode() => Value;
    }

    internal class T1: IEquatable<T1>
    {
        internal int Value { get; }

        internal T1(int value)
        {
            Value = value;
        }

        public override string ToString() => $"{nameof(T1)}({Value})";

        public bool Equals(T1 other) => other != null && Value == other.Value;

        public override bool Equals(object obj) => obj is T1 other && Value == other.Value;

        public override int GetHashCode() => Value;
    }

    internal class E0: IEquatable<E0>
    {
        internal int Value { get; }

        internal E0(int value)
        {
            Value = value;
        }

        public override string ToString() => $"{nameof(E0)}({Value})";

        public bool Equals(E0 other) => other != null && Value == other.Value;

        public override bool Equals(object obj) => obj is E0 other && Value == other.Value;

        public override int GetHashCode() => Value;
    }

    internal class E1: IEquatable<E1>
    {
        internal int Value { get; }

        internal E1(int value)
        {
            Value = value;
        }

        public override string ToString() => $"{nameof(E1)}({Value})";

        public bool Equals(E1 other) => other != null && Value == other.Value;

        public override bool Equals(object obj) => obj is E1 other && Value == other.Value;

        public override int GetHashCode() => Value;
    }

    public class ResultTests
    {
        [Fact]
        public void NotNullOrWorks(){
            Result00.NotNullOr(new T0(0), new E0(0)).Should().Be(Result00.Ok(new T0(0)));
            Result00.NotNullOr(null, new E0(0)).Should().Be(Result00.Err(new E0(0)));
        }

        [Fact]
        public void NotNullOrElseWorks(){
            Result00.NotNullOrElse(new T0(0), () => new E0(0)).Should().Be(Result00.Ok(new T0(0)));
            Result00.NotNullOrElse(null, () => new E0(0)).Should().Be(Result00.Err(new E0(0)));
        }

        [Fact]
        public void AndWorks()
        {
            Result00.Ok(new T0(0)).And(Result10.Ok(new T1(1))).Should().Be(Result10.Ok(new T1(1)));
            Result00.Ok(new T0(0)).And(Result10.Err(new E0(1))).Should().Be(Result10.Err(new E0(1)));
            Result00.Err(new E0(0)).And(Result10.Ok(new T1(1))).Should().Be(Result10.Err(new E0(0)));
            Result00.Err(new E0(0)).And(Result10.Err(new E0(1))).Should().Be(Result10.Err(new E0(0)));
        }

        [Fact]
        public void AndThenWorks()
        {
            Result00.Ok(new T0(0)).AndThen(_ => Result10.Ok(new T1(1))).Should().Be(Result10.Ok(new T1(1)));
            Result00.Ok(new T0(0)).AndThen(_ => Result10.Err(new E0(1))).Should().Be(Result10.Err(new E0(1)));
            Result00.Err(new E0(0)).AndThen(_ => Result10.Ok(new T1(1))).Should().Be(Result10.Err(new E0(0)));
            Result00.Err(new E0(0)).AndThen(_ => Result10.Err(new E0(1))).Should().Be(Result10.Err(new E0(0)));
        }
        [Fact]
        public void OrWorks()
        {
            Result00.Ok(new T0(0)).Or(Result01.Ok(new T0(1))).Should().Be(Result01.Ok(new T0(0)));
            Result00.Ok(new T0(0)).Or(Result01.Err(new E1(1))).Should().Be(Result01.Ok(new T0(0)));
            Result00.Err(new E0(0)).Or(Result01.Ok(new T0(1))).Should().Be(Result01.Ok(new T0(1)));
            Result00.Err(new E0(0)).Or(Result01.Err(new E1(1))).Should().Be(Result01.Err(new E1(1)));
        }

        [Fact]
        public void OrElse()
        {
            Result00.Ok(new T0(0)).OrElse(_ => Result01.Ok(new T0(1))).Should().Be(Result01.Ok(new T0(0)));
            Result00.Ok(new T0(0)).OrElse(_ => Result01.Err(new E1(1))).Should().Be(Result01.Ok(new T0(0)));
            Result00.Err(new E0(0)).OrElse(_ => Result01.Ok(new T0(1))).Should().Be(Result01.Ok(new T0(1)));
            Result00.Err(new E0(0)).OrElse(_ => Result01.Err(new E1(1))).Should().Be(Result01.Err(new E1(1)));
        }

        [Fact]
        public void MapWorks()
        {
            static T1 F(T0 t0) => new T1(t0.Value + 1);
            Result00.Ok(new T0(0)).Map(F).Should().Be(Result10.Ok(new T1(1)));
            Result00.Err(new E0(0)).Map(F).Should().Be(Result10.Err(new E0(0)));
        }

        [Fact]
        public void MapErrWorks()
        {
            static E1 F(E0 e0) => new E1(e0.Value + 1);
            Result00.Ok(new T0(0)).MapErr(F).Should().Be(Result01.Ok(new T0(0)));
            Result00.Err(new E0(0)).MapErr(F).Should().Be(Result01.Err(new E1(1)));
        }

        [Fact]
        public void UnwrapWorks()
        {
            Result00.Ok(new T0(0)).Unwrap().Should().Be(new T0(0));
            Result00.Err(new E0(0)).Invoking(result => result.Unwrap()).Should().Throw<ResultUnwrapException<E0>>()
                .And.Inner.Should().Be(new E0(0));
        }

        [Fact]
        public void UnwrapOrWorks()
        {
            Result00.Ok(new T0(0)).UnwrapOr(new T0(1)).Should().Be(new T0(0));
            Result00.Err(new E0(0)).UnwrapOr(new T0(1)).Should().Be(new T0(1));
        }

        [Fact]
        public void UnwrapOrElseWorks()
        {
            Result00.Ok(new T0(0)).UnwrapOrElse(_ => new T0(1)).Should().Be(new T0(0));
            Result00.Err(new E0(0)).UnwrapOrElse(_ => new T0(1)).Should().Be(new T0(1));
        }
    }
}
