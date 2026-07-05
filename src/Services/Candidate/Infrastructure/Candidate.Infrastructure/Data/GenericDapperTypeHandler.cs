using Candidate.Domain.Abstractions;
using Dapper;
using System.Data;

namespace Candidate.Infrastructure.Data
{
	public class GenericDapperTypeHandler<TVO> : SqlMapper.ITypeHandler
		where TVO : IValueObject<TVO>
	{
		public void SetValue(IDbDataParameter parameter, object value)
		{
			// treat DBNull the same as null
			if (value == null || value is DBNull)
			{
				parameter.Value = DBNull.Value;
				parameter.DbType = DbType.String;
				return;
			}

			// If it's already the VO
			if (value is IValueObject<TVO> vo)
			{
				parameter.Value = vo.Value ?? (object)DBNull.Value;
				parameter.DbType = DbType.String;
				return;
			}

			// If caller provided raw string or other convertible value
			var s = value.ToString();
			parameter.Value = string.IsNullOrEmpty(s) ? (object)DBNull.Value : s;
			parameter.DbType = DbType.String;
		}

		public object? Parse(Type destinationType, object value)
		{
			if (value == null || value is DBNull)
				return null;

			var stringValue = value?.ToString() ?? throw new DataException($"Cannot convert null to {nameof(TVO)}.");
			return TVO.FromString(stringValue);
		}
	}
}
