//
//  NativeUnityPlugin.m
//  Ifeelgame
//
//  Created by ifeel on 2020/8/27.
//  Copyright © 2020 ifeel. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <NativeAPI.h>
#import "NativeUnityPlugin.h"

@implementation NativeUnityPlugin

char* toUnityStr(const char* str)
{
    if (!str)
        return NULL;
    return strcpy((char*)malloc(strlen(str) + 1), str);
}

extern "C" {    
    const char* mGetRegionName(){
        return toUnityStr([[[NativeAPI Instance] GetRegionName] UTF8String]);
    }
    
    const char* mGetLanguage(){
        return toUnityStr([[[NativeAPI Instance] GetLanguage] UTF8String]);
    }
    
    const char* mGetUUID(){
        return toUnityStr([[[NativeAPI Instance] GetUUID] UTF8String]);
    }
}

@end
