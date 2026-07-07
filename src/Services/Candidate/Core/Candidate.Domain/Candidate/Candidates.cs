using Common.Platform.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candidate.Domain.Candidate
{
	public class Candidates : IEntity
	{
		public Candidates(
			long id,
			long userId,
			string firstName,
			string lastName,
			string email,
			string? phone,
			string? address,
			string? linkedinProfile,
			string? portfolioUrl,
			string? headline,
			string? summary,
			bool isActive,
			string? createdBy,
			DateTime? updatedOn,
			string? updatedBy)
		{
			Id = id;
			UserId = userId;
			FirstName = firstName;
			LastName = lastName;
			Email = email;
			Phone = phone;
			Address = address;
			LinkedinProfile = linkedinProfile;
			PortfolioUrl = portfolioUrl;
			Headline = headline;
			Summary = summary;
			IsActive = isActive;

			CreatedBy = createdBy;
			UpdatedOn = updatedOn;
			UpdatedBy = updatedBy;
		}

		public Candidates()
		{
		}

		public string GetPrimaryKeyName() => nameof(Id);

		public long GetPrimaryKeyValue() => Id;

		public long Id { get; set; }

		public long UserId { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Email { get; set; }

		public string? Phone { get; set; }

		public string? Address { get; set; }

		public string? LinkedinProfile { get; set; }

		public string? PortfolioUrl { get; set; }

		public string? Headline { get; set; }

		public string? Summary { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedOn { get; set; }

		public string? CreatedBy { get; set; }

		public DateTime? UpdatedOn { get; set; }

		public string? UpdatedBy { get; set; }

		public static Candidates Create(
			long userId,
			string firstName,
			string lastName,
			string email,
			string? phone,
			string? address,
			string? linkedinProfile,
			string? portfolioUrl,
			string? headline,
			string? summary,
			string? createdBy)
		{
			return new Candidates(
				id: 0,
				userId: userId,
				firstName: firstName,
				lastName: lastName,
				email: email,
				phone: phone,
				address: address,
				linkedinProfile: linkedinProfile,
				portfolioUrl: portfolioUrl,
				headline: headline,
				summary: summary,
				isActive: true,
				createdBy: createdBy,
				updatedOn: null,
				updatedBy: null);
		}

		public void Update(
			string firstName,
			string lastName,
			string email,
			string? phone,
			string? address,
			string? linkedinProfile,
			string? portfolioUrl,
			string? headline,
			string? summary,
			bool isActive,
			string? updatedBy)
		{
			FirstName = firstName;
			LastName = lastName;
			Email = email;
			Phone = phone;
			Address = address;
			LinkedinProfile = linkedinProfile;
			PortfolioUrl = portfolioUrl;
			Headline = headline;
			Summary = summary;
			IsActive = isActive;

			UpdatedBy = updatedBy;
			UpdatedOn = DateTime.Now;
		}
	}
}
