
  BEGIN TRANSACTION
  SET IDENTITY_INSERT webpages_Roles ON;

  INSERT INTO webpages_Roles(RoleId, RoleName)
  VALUES(1, 'GroupManager')

  INSERT INTO webpages_Roles(RoleId, RoleName)
  VALUES(2, 'Admin')

  SET IDENTITY_INSERT webpages_Roles OFF;
  COMMIT TRANSACTION
