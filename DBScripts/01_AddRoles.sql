


  BEGIN TRANSACTION
  SET IDENTITY_INSERT webpages_Roles ON;

  INSERT INTO webpages_Roles(RoleId, RoleName)
  VALUES(1, 'GroupManager')

  SET IDENTITY_INSERT webpages_Roles OFF;
  COMMIT TRANSACTION
