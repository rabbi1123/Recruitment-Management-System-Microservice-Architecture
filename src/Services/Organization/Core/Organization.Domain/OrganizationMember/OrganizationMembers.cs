using Common.Platform.Domain.Abstractions;

namespace Organization.Domain.OrganizationMember
{
	public class OrganizationMembers : IEntity
	{
		public OrganizationMembers(
			long id,
			long organizationId,
			long userId,
			string fullName,
			string? email,
			string? phone,
			string role,
			bool isActive,
			string? createdBy,
			DateTime? updatedOn,
			string? updatedBy)
		{
			Id = id;
			OrganizationId = organizationId;
			UserId = userId;
			FullName = fullName;
			Email = email;
			Phone = phone;
			Role = role;
			IsActive = isActive;

			CreatedBy = createdBy;
			UpdatedOn = updatedOn;
			UpdatedBy = updatedBy;
		}

		public OrganizationMembers()
		{
		}

		public string GetPrimaryKeyName() => nameof(Id);

		public long GetPrimaryKeyValue() => Id;

		public long Id { get; set; }

		public long OrganizationId { get; set; }

		public long UserId { get; set; }

		public string FullName { get; set; }

		public string? Email { get; set; }

		public string? Phone { get; set; }

		public string Role { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedOn { get; set; }

		public string? CreatedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public string? UpdatedBy { get; set; }

		public static OrganizationMembers Create(
			long organizationId,
			long userId,
			string fullName,
			string? email,
			string? phone,
			string role,
			string? createdBy)
		{
			return new OrganizationMembers(
				id: 0,
				organizationId: organizationId,
				userId: userId,
				fullName: fullName,
				email: email,
				phone: phone,
				role: role,
				isActive: true,
				createdBy: createdBy,
				updatedOn: null,
				updatedBy: null);
		}

		public void Update(
			string fullName,
			string? email,
			string? phone,
			string role,
			bool isActive,
			string? updatedBy)
		{
			FullName = fullName;
			Email = email;
			Phone = phone;
			Role = role;
			IsActive = isActive;

			UpdatedBy = updatedBy;
			UpdatedOn = DateTime.Now;
		}
	}
}
