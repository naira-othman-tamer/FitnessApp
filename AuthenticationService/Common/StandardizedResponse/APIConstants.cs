namespace AuthenticationService.Common.StandardizedResponse
{
    public static partial class APIConstants
    {
        public static class APIMessages
        {
            public const string Success = "The operation was successful";
            public const string SuccessLocalized = "تمت العملية بنجاح";
            public const string NoContent = "No Data Found";
            public const string NoContentLocalized = "لم يتم أيجاد بيانات";
            public const string Error = "The operation failed!";
            public const string ErrorLocalized = "فشلت العملية";
            public const string BadRequest = "Invalid request!";
            public const string BadRequestLocalized = "بيانات الطلب غير صحيحة";
            public const string NotFound = "No data found!";
            public const string NotFoundLocalized = "لم يتم العثور على البيانات";
            public const string DataCorruptionLocalized = "هنالك خطأ في شكل المعلومات المبعوثة";
            public const string Forbidden = "You don't have permission to perform this operation!";
            public const string ForbiddenLocalized = "ليس لديك إذن لأداء هذه العملية";
            public const string Conflict = "This already is found";
            public const string ConflictLocalized = "هذا موجود بالفعل";
        }

        public static class UserMessages
        {
            public const string NotFound = "User not found!";
            public const string NotFoundLocalized = "لم يتم العثور على المستخدم";
            public const string NotActive = "User account is not active!";
            public const string NotActiveLocalized = "حساب المستخدم غير نشط";
            public const string AlreadyExist = "A user with the same mobile number already exist!";
            public const string AlreadyExistLocalized = "رقم الهاتف مُسجل بالفعل";
            public const string IncompleteProfile = "An existing record was found with incomplete profile. Kindly update your information to proceed.";
            public const string IncompleteProfileLocalized = "تم العثور على المستخدم ولكن الملف الشخصي غير مكتمل. يُرجى تحديث معلوماتك للمتابعة";
            public const string Resubmission = "Your registration has already been reviewed. Please update your profile as requested by the administrator to proceed.";
            public const string ResubmissionLocalized = "تم مراجعة تسجيلك. يُرجى تحديث ملفك الشخصي بناءً على طلب المسؤول للمتابعة";
            public const string OwnerNotFound = "You are not a member of the Palm Hills community!";
            public const string OwnerNotFoundLocalized = "أنت لست عضوًا في مجتمع بالم هيلز";
            public const string InvalidStatus = "User status does not support this operation!";
            public const string InvalidStatusLocalized = "حالة المستخدم لا تسمح بهذة العملية";
            public const string InvalidOTP = "Invalid OTP!";
            public const string InvalidOTPLocalized = "الرقم المُتغير غير صحيح";
            public const string OTPExpired = "OTP expired!";
            public const string OTPExpiredLocalized = "انتهت صلاحية رمز التحقق";
            public const string InvalidPassword= "Please enter the password again !";
            public const string InvalidPasswordLocalized = "! يرجى اعادة ادخال كلمة المرور ";
            public const string NotVerified = "Your account isn't verified yet";
            public const string NotVerifiedLocalized = "الحساب الخاص بك غير موثق حتى الأن";
            public const string Suspended = "Too many Requests ! Your account is suspended for 15 min.";
            public const string SuspendedLocalized = "كثير من المحاولات ! حسابك معلق لمدة 15 دقيقة";
        }
    }
}
