from enum import  Enum

class CheckResult(Enum):
	CHANGED = 1
	UNCHANGED = 2
	ERROR = 3
	INACTIVE = 4

class MessageType(Enum):
	DIFF = 1
	URL = 2
	CUSTOM = 3

class Status(Enum):
	ENABLED = 1
	DISABLED = 2
	ERROR = 3

class UpdateBaselineFlag(Enum):
	ALWAYS = 1
	ONLY_ON_ERROR = 2
	ONLY_ON_CHANGED = 3
	ON_CHANGED_AND_ERROR = 4
	NEVER = 5