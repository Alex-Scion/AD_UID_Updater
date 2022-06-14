# AD_UID_Updater
Updates UID of Active Directory objects

Simple form for updating AD objects' UIDs. When RFC2307 is enabled on AD, every AD object (users and groups) get 
another field (among other things) where UID can be stored. As a reminder, UID is user's identification in a
UNIX controlled domain (like LDAP) and SID is users identification from AD. After RFC2307 enabled, all AD objects
get blank UID fields. In the ADs with many users, assigning these individually becomes a problem, hence this form. 

This form can read UIDs from the AD and when assigning new UIDs, ensures that UIDs are unique. No special pattern 
is being followerd for defining UIDs (this could be improved in the next push). however, writting to AD hasn't 
been tested as the developer haven't had write access to the AD. This needs testing.

