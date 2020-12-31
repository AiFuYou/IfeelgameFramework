//
//  NativeAPI.h
//  Ifeelgame
//
//  Created by ifeel on 2020/8/27.
//  Copyright © 2020 ifeel. All rights reserved.
//

#ifndef NativeAPI_h
#define NativeAPI_h

@interface NativeAPI : NSObject

+ (NativeAPI*) Instance;
- (NSString*) GetRegionName;
- (NSString*) GetLanguage;
- (NSString*) GetUUID;

@end

#endif /* NativeAPI_h */
