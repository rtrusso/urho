#download clang 3.7 from here: http://llvm.org/releases/download.html 
CUSTOM_CLANG=/tools/clang370/bin/clang

Android:
	make libmono-urho.so -f MakeAndroid ABI="armeabi" && make libmono-urho.so -f MakeAndroid ABI="armeabi-v7a" && make libmono-urho.so -f MakeAndroid ABI="x86"
	
Mac:
	make libmono-urho.dylib -f MakeMac
	
iOS:
	make fat-libmono-urho.dylib -f MakeiOS
	
Windows:
	make libUrho3D.a -f MakeWindows
	
All-Macos: Android Mac iOS

All-Windows: Android Windows

#compile Urho.pch for SharpieBinder on Mac
PchMac:
	make Urho3D_Mac -f MakeMac && $(CUSTOM_CLANG) -cc1 -emit-pch -o bindings/Urho.pch bindings/all-urho.cpp  -I$(URHO_MAC_DIR)/include -I$(URHO_MAC_DIR)/include/Urho3D/ThirdParty

ParseEventsMac:
	perl parse.pl $(URHO_DIR)/include/Urho3d/*/*h