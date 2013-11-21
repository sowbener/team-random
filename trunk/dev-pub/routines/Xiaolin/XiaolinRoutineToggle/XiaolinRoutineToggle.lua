
RoutineToggles = {
	elusivebrewStatus = false;
	autodizzlingStatus = false;
	aoeStatus = false;
	cooldownStatus = false;
	pauseStatus = false;
	
}
	 
SLASH_CDTOGGLE1 = '/MyCooldowns';
function SlashCmdList.CDTOGGLE(msg, editbox) -- 4.
	if msg == "true" then
		RoutineToggles.cooldownStatus = true;
	end
	if msg == "false" then
		RoutineToggles.cooldownStatus = false;
	end
end

SLASH_CDTOGGLE1 = '/Def';
function SlashCmdList.CDTOGGLE(msg, editbox) -- 4.
	if msg == "true" then
		RoutineToggles.pauseStatus = true;
	end
	if msg == "false" then
		RoutineToggles.pauseStatus = false;
	end
end


SLASH_AOETOGGLE1 = '/AoEderp';
function SlashCmdList.AOETOGGLE(msg, editbox) -- 4.
	if msg == "true" then
		RoutineToggles.aoeStatus = true;
	end
	if msg == "false" then
		RoutineToggles.aoeStatus = false;
	end
end

SLASH_MOVEMENTTOGGLE1 = '/elusivebrew';
function SlashCmdList.MOVEMENTTOGGLE(msg, editbox) -- 4.
	if msg == "true" then
		RoutineToggles.elusivebrewStatus = true;
	end
	if msg == "false" then
		RoutineToggles.elusivebrewStatus = false;
	end
end

SLASH_TARGETINGTOGGLE1 = '/dizzling';
function SlashCmdList.TARGETINGTOGGLE(msg, editbox) -- 4.
	if msg == "true" then
		RoutineToggles.autodizzlingStatus = true;
	end
	if msg == "false" then
		RoutineToggles.autodizzlingStatus = false;
	end
end


function RoutineToggles_OnUpdate(self, elapsed) 
	self.TimeSinceLastUpdate = self.TimeSinceLastUpdate + elapsed;
	local cStatus = "|cffFF0000Disabled";
	local adStatus = "|cffFF0000Disabled";
	local aStatus = "|cffFF0000Disabled";
	local ebStatus = "|cffFF0000Disabled";
	local pStatus = "|cffFF0000Disabled";
		
	if (RoutineToggles.cooldownStatus == true) then
		cStatus = "|cff00FF00Enabled";
	end
	if (RoutineToggles.pauseStatus == true) then
	    pStatus = "cff00FF00Enabled";
	end
	if (RoutineToggles.autodizzlingStatus == true) then
		adStatus = "|cff00FF00Enabled";
	end
	if (RoutineToggles.aoeStatus == true) then
		aStatus = "|cff00FF00Enabled";
	end
	if (RoutineToggles.elusivebrewStatus == true) then
		ebStatus = "|cff00FF00Enabled";
	end

	
	if (self.TimeSinceLastUpdate > 1.0) then
		Xiaolin_FrameText:SetText("[Xiaolin' Combat Routines]\n|cffFFFFFFElusiveBrew: "..ebStatus.."\n|cffFFFFFFCooldowns: "..cStatus.."\n|cffFFFFFFAuto-Dizzling: "..adStatus.."\n|cffFFFFFFAoE: "..aStatus.."\n"|cffFFFFFFPause: "..pStatus.."\n);
	end
end 	



