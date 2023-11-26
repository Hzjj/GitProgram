using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text;
namespace WebService.Util
{
    /// <summary>
    /// 通用数据类
    /// </summary>
    public class Data
    {
        SqlHepler sql = new SqlHepler();

        public bool SelDz(string his_id, string type_name)
        {
            bool b = false;
            try
            {
                string selstr = "select his_id from futian_user.TJ_HISDZB where his_id=@his_id and type_name=@type_name";
                SqlParameter[] par =
                {
                    new SqlParameter("his_id",SqlDbType.VarBinary,200),
                    new SqlParameter("type_name",SqlDbType.VarBinary,30)
                };
                par[0].Value = his_id;
                par[1].Value = type_name;
                if (SqlHepler.QueryTable(selstr, par).Rows.Count > 0)
                {
                    b = true;
                }
                else
                {
                    b = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return b;
        }

        public bool AddHisDz(string his_id, string his_name, string type_name)
        {
            bool b = false;
            try
            {
                string selstr = "insert into futian_user.TJ_HISDZB(his_id,his_name,type_name,CreateTime) values(@his_id,@his_name,@type_name,GETdate())";
                SqlParameter[] par =
                {
                    new SqlParameter("his_id",SqlDbType.VarBinary,200),
                    new SqlParameter("his_name",SqlDbType.VarBinary,400),
                    new SqlParameter("type_name",SqlDbType.VarBinary,30)
                };
                par[0].Value = his_id;
                par[1].Value = his_name;
                par[2].Value = type_name;
                if (SqlHepler.ExecuteSql(selstr, par) > 0)
                {
                    b = true;
                }
                else
                {
                    b = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return b;
        }

        public bool UpdHisDz(string his_id, string his_name, string type_name)
        {
            bool b = false;
            try
            {
                string selstr = "Update futian_user.TJ_HISDZB set his_name=@his_name where his_id=@his_id and type_name=@type_name ";
                SqlParameter[] par =
                {
                    new SqlParameter("his_id",SqlDbType.VarBinary,200),
                    new SqlParameter("his_name",SqlDbType.VarBinary,400),
                    new SqlParameter("type_name",SqlDbType.VarBinary,30)
                };
                par[0].Value = his_id;
                par[1].Value = his_name;
                par[2].Value = type_name;
                if (SqlHepler.ExecuteSql(selstr, par) > 0)
                {
                    b = true;
                }
                else
                {
                    b = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return b;
        }


        /// <summary>
        /// 获取个人基本信息
        /// </summary>
        /// <param name="tjbh"></param>
        /// <param name="tjcs"></param>
        /// <returns></returns>
        public DataTable GetGrxx(string tjbh = "", int tjcs = 0, string Djlsh = "")
        {
            DataTable dt = new DataTable();
            string sm = string.Format("体检编号:{0},体检次数:{1},登记流水号:{2}", tjbh, tjcs, Djlsh);
            try
            {
                Dictionary<string, string> sex = new Dictionary<string, string>
                {
                    { "0","女"},
                    { "1","男"},
                    { "%","未知"},

                };
                Dictionary<string, string> sexCode = new Dictionary<string, string>
                {
                    { "0","0"},
                    { "1","1"},
                     { "%","%"},
                };

                StringBuilder str = new StringBuilder();
                str.Append(" select d.MZMC,a.nl,C.MC,a.HIS_PatientId,a.HIS_ClinicNo,GZYR.BZ,a.TJBH,a.XM,CSNYR,SFZH,ADDRESS,ISNULL(MOBILE,PHONE) AS sjhm,a.DWBH ");
                str.Append(",(CASE WHEN a.HYZK = '0' THEN '未婚' WHEN a.HYZK = '1' THEN '已婚' ELSE '未知' END) AS HYZK");
                str.Append(",(case when a.XB='0' then '"+ sex["0"] + "' WHEN a.XB='1' THEN '"+ sex["1"] + "'  ELSE '"+ sex["%"] + "' end) as sex ");
                str.Append(",(case when a.XB='0' then '"+ sexCode["0"] + "' WHEN a.XB='1' THEN '"+ sexCode["1"] + "'  ELSE '"+ sexCode["%"] + "' end) as sexCode ");
                str.Append(" from futian_user.TJ_TJDJB a (nolock) left join futian_user.GZRY GZYR (nolock) on a.DJRY = GZYR.GKHM left join futian_user.HYDWDMB c (nolock) on a.DWBH=c.BH  left join futian_user.TJ_DMB_MZ d (nolock) on a.MZ=d.SZCode   where ");
                if (string.IsNullOrEmpty(tjbh) || tjcs == 0)
                {
                    str.Append(" Djlsh=@Djlsh");
                }
                else
                {
                    str.Append(" tjbh=@tjbh and tjcs=@tjcs");
                }
                SqlParameter[] par =
                {
                    new SqlParameter("tjbh",SqlDbType.VarChar,14),
                    new SqlParameter("tjcs",SqlDbType.Int),
                    new SqlParameter("Djlsh",SqlDbType.Char,12)
                };
                par[0].Value = tjbh;
                par[1].Value = tjcs;
                par[2].Value = Djlsh;
                dt = SqlHepler.QueryTable(str.ToStr(), par);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    throw new Exception("未获取" + sm + "的个人基本信息，请检查输入的用户编码是否正确");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("在获取" + sm + "个人基本信息时出现异常错误,错误源为:" + ex.Message.ToStr());
            }
        }

        /// <summary>
        /// 获取个人项目信息
        /// </summary>
        /// <param name="tjbh"></param>
        /// <param name="tjcs"></param>
        /// <returns></returns>
        public DataTable GetXm(string Zhxmbh, string tjbh = "", int tjcs = 0, string Djlsh = "")
        {
            DataTable dt = new DataTable();
            string sm = string.Format("体检编号:{0},体检次数:{1},登记流水号:{2}", tjbh, tjcs, Djlsh);
            try
            {
                string str = @"select distinct XMDZ.HIS_SFDM,DJ.his_patid,XMDZ.HIS_SFXMMC ,
                DJ.MZ,DJ.JCYS,DJ.YSBH,DJ.DJRQ,JL.TJBH,DJ.SFZH,ZHXM.JCJYLX,jl.xmdj,JL.SSDJ,
                --工作人员
                GZYR.XM as CZY,DJ.DJRY,
                --项目信息
                JL.TJCS,JL.TJBH,JL.BARCODE,JL.TJXMBH ,JL.his_cfxh,JL.his_cfmxxh,
                JL.SQDH,DJ.JCRQ,JL.YSBH,JL.JCYS,XMDZ.ZXKSID as ZXKS,
                XMDZ.ZXKSMC,DJ.CZY
                from futian_user.TJ_TJDJB DJ
                left join futian_user.TJ_TJJLB JL on DJ.TJBH = JL.TJBH and DJ.TJCS = JL.TJCS               
                left join futian_user.TJ_HISJK_XMDZB XMDZ on XMDZ.XMBH = JL.TJXMBH
                left join futian_user.GZRY GZYR on DJ.DJRY = GZYR.GKHM
                left join futian_user.TJ_ZHXM_HD ZHXM on ZHXM.BH = JL.TJXMBH 
                left join futian_user.TJ_TJLXB LX on LX.LXBH = ZHXM.ZXKS where JL.tjxmbh=@Zhxmbh";
                if (string.IsNullOrEmpty(tjbh) || tjcs == 0)
                {
                    str += " and Djlsh=@Djlsh";
                }
                else
                {
                    str += " and tjbh=@tjbh and tjcs=@tjcs";
                }

                SqlParameter[] par =
                {
                    new SqlParameter("tjbh",SqlDbType.VarChar,14),
                    new SqlParameter("tjcs",SqlDbType.Int),
                    new SqlParameter("Djlsh",SqlDbType.Char,12),
                    new SqlParameter("Zhxmbh",SqlDbType.Char,6)
                };
                par[0].Value = tjbh;
                par[1].Value = tjcs;

                par[2].Value = Djlsh;
                par[3].Value = Zhxmbh;
                dt = SqlHepler.QueryTable(str.ToStr(), par);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    throw new Exception("未获取" + sm + "的个人项目信息");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("在获取" + sm + "个人项目时出现异常错误,错误源为:" + ex.ToStr());
            }
        }

        /// <summary>
        /// 获取个人收费项目信息
        /// </summary>
        /// <param name="tjbh"></param>
        /// <param name="tjcs"></param>
        /// <returns></returns>
        public DataTable GetSFXm(string tjbh = "", int tjcs = 0, string Djlsh = "", string Zhxmbh = "")
        {
            DataTable dt = new DataTable();
            string sm = string.Format("体检编号:{0},体检次数:{1},登记流水号:{2}", tjbh, tjcs, Djlsh);
            try
            {
                string str = @"select distinct JL.TJXMBH,ZHXM.MC,JL.SSDJ,JL.XMDJ,LX.BZ AS ZXKS,LX.MC AS KSMC,
                left(DJ.dwbh,4) as hydwbh
                from  futian_user.TJ_TJJLB JL 
                left join futian_user.TJ_TJDJB DJ on JL.TJBH=DJ.TJBH AND JL.TJCS=DJ.TJCS
				left join futian_user.TJ_ZHXM_HD ZHXM on ZHXM.BH=JL.TJXMBH
                left join futian_user.TJ_TJLXB LX on LX.LXBH = JL.ZXKS
                left join futian_user.HYDWDMB dwb on dwb.bh=left(dj.dwbh,4)
                where LEN(TJXMBH)>2 and JL.TJXMBH=ISNULL(@Zhxmbh,JL.TJXMBH)  ";
                if (string.IsNullOrEmpty(tjbh) || tjcs == 0)
                {
                    str += " and Djlsh=@Djlsh";
                }
                else
                {
                    str += " and DJ.tjbh=@tjbh and DJ.tjcs=@tjcs";
                }


                SqlParameter[] par =
                {
                    new SqlParameter("tjbh",SqlDbType.VarChar,14),
                    new SqlParameter("tjcs",SqlDbType.Int),
                    new SqlParameter("Djlsh",SqlDbType.Char,12),
                    new SqlParameter("Zhxmbh",SqlDbType.Char,6)
                };
                par[0].Value = tjbh;
                par[1].Value = tjcs;

                par[2].Value = Djlsh;
                if (string.IsNullOrEmpty(Zhxmbh))
                {
                    par[3].Value = DBNull.Value;
                }
                else
                {
                    par[3].Value = Zhxmbh.ToStr();
                }
                dt = SqlHepler.QueryTable(str.ToStr(), par);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    throw new Exception("未获取" + sm + "的个人项目信息");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("在获取" + sm + "个人项目信息时出现异常错误,错误源为:" + ex.ToStr());
            }
        }

        /// <summary>
        /// 获取对照项目的收费代码
        /// </summary>
        /// <param name="tjbh"></param>
        /// <param name="tjcs"></param>
        /// <returns></returns>
        public DataTable GetHISXm(string Zhxmbh)
        {
            DataTable dt = new DataTable();
            string sm = string.Format("组合项目编号:{0}", Zhxmbh);
            try
            {
                string str = @"	select HIS_SFDM,HIS_SFXMMC,DJ,SL from futian_user.TJ_HISJK_XMDZB 
                                where XMBH=@xmbh  ";

                SqlParameter[] par =
                {
                    new SqlParameter("xmbh",SqlDbType.VarChar,20)
                };
                par[0].Value = Zhxmbh;

                dt = SqlHepler.QueryTable(str.ToStr(), par);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    throw new Exception("" + sm + "未对照！");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("在获取" + sm + "对照项目信息时出现异常错误,错误源为:" + ex.ToStr());
            }
        }

        /// <summary>
        /// 获取民族名称
        /// </summary>
        /// <param name="tjbh"></param>
        /// <param name="tjcs"></param>
        /// <returns></returns>
        public string GetMZ(string MZ)
        {
            string xm = "";
            string sm = string.Format("民族代码:{0}", MZ);
            try
            {
                string str = "  SELECT MZMC FROM futian_user.TJ_DMB_MZ where SZCODE=@SZCODE";

                SqlParameter[] par =
                {
                    new SqlParameter("SZCODE",SqlDbType.SmallInt)
                };
                par[0].Value = MZ.ToInt();
                DataTable dt = new DataTable();
                dt = SqlHepler.QueryTable(str.ToStr(), par);
                if (dt.Rows.Count > 0)
                {
                    return dt.AsEnumerable().First()[0].ToStr();
                }
                else
                {
                    return "汉族";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("在获取" + sm + "姓名时出现异常错误,错误源为:" + ex.ToStr());
            }
        }
        public DataTable GetXm(string zhxmbhS, string Djlsh = "")
        {
            DataTable dt = new DataTable();
            string sm = string.Format("登记流水号:{0}", Djlsh);
            try
            {
                //string str = @"exec('select distinct XMDZ.HIS_SFDM,DJ.HIS_PatientId,DJ.HIS_ClinicNo,XMDZ.HIS_SFXMMC ,
                //    DJ.MZ,DJ.JCYS,DJ.YSBH,DJ.DJRQ,JL.TJBH,DJ.SFZH,ZHXM.JCJYLX,jl.xmdj,JL.SSDJ,DJ.MC,SFZH,(case when DJ.XB=''0'' then ''2''   ELSE ''9'' end) as sex,(case when DJ.XB=''0'' then ''女'' when DJ.XB=''1'' then ''男''   ELSE ''未说明性别'' end) as sexMc ,DJ.NL,DJ.CSNYR,    
                //    ISNULL(MOBILE,PHONE) AS sjhm,
                //    --工作人员
                //    GZYR.XM as CZY,DJ.DJRY,
                //    --项目信息
                //    JL.TJCS,JL.TJBH,JL.BARCODE,JL.TJXMBH ,JL.his_cfxh,JL.his_cfmxxh,JL.XH, 
                //    JL.SQDH,DJ.JCRQ,JL.YSBH,JL.JCYS,XMDZ.ZXKSID as ZXKS, XMDZ.ZXKSMC
                //    from futian_user.TJ_TJDJB DJ
                //    left join futian_user.TJ_TJJLB JL on DJ.TJBH = JL.TJBH and DJ.TJCS = JL.TJCS                
                //    left join futian_user.TJ_HISJK_XMDZB XMDZ on XMDZ.XMBH = JL.TJXMBH 
                //    left join futian_user.GZRY GZYR on DJ.DJRY = GZYR.GKHM 
                //    left join futian_user.TJ_ZHXM_HD ZHXM on ZHXM.BH = JL.TJXMBH  
                //    left join futian_user.TJ_TJLXB LX on LX.LXBH = ZHXM.ZXKS where Djlsh='+@Djlsh+' ";
                //20230113 zj edit 1nan2nv3bx  add  LX.RecDeptCode,LX.RecDeptDesc,ZHXM.ClassCode,ZHXM.SubClassCode,ZHXM.BodyPart
                string str = @"exec('select distinct djlsh,XMDZ.GJC, XMDZ.GJC as HIS_SFDM,XMDZ.GJCMC as HIS_SFXMMC ,DJ.DJLSH,DJ.RYBH,DJ.XM,
                DJ.MZ,DJ.JCYS,DJ.YSBH,DJ.DJRQ,JL.TJBH,DJ.SFZH,ZHXM.JCJYLX,jl.xmdj,JL.SSDJ,(case when DJ.XB=''0'' then ''2''  when DJ.XB=''1'' then ''1''  ELSE ''3'' end) as sex,(case when DJ.XB=''0'' then ''女'' when DJ.XB=''1'' then ''男''   ELSE ''不详'' end) as sexMc ,DJ.NL,DJ.CSNYR,    
                ISNULL(MOBILE,PHONE) AS sjhm,DJ.address,
                --工作人员
                GZYR.XM as CZY,DJ.DJRY,
                --项目信息
                JL.TJCS,JL.TJBH,JL.BARCODE,JL.TJXMBH ,JL.XH, 
                JL.SQDH,DJ.JCRQ,JL.YSBH,JL.JCYS,ZHXM.ZXKS,LX.MC as ZXKSMC               
                
                from futian_user.TJ_TJDJB DJ
                left join futian_user.TJ_TJJLB JL on DJ.TJBH = JL.TJBH and DJ.TJCS = JL.TJCS                
                left join futian_user.TJ_ZHXMDZB XMDZ on XMDZ.TJZHXM = JL.TJXMBH 
                left join futian_user.GZRY GZYR on DJ.DJRY = GZYR.GKHM 
                left join futian_user.TJ_ZHXM_HD ZHXM on ZHXM.BH = JL.TJXMBH  
                left join futian_user.TJ_TJLXB LX on LX.LXBH = ZHXM.ZXKS  
			    where Djlsh='+@Djlsh+' ";
                //2023 04 15 zy modify from LX.RecDeptCode, to ZHXM.gnlx as RecDeptCode
                if (!string.IsNullOrEmpty(zhxmbhS))
                {
                    str += " and JL.TJXMBH in ('+@Zhxmbh+')')";
                }
                else
                {
                    str += " ')";
                }

                SqlParameter[] par =
                {
                    new SqlParameter("Djlsh",SqlDbType.Char,12),
                    new SqlParameter("Zhxmbh",SqlDbType.VarChar,200)
                };
                par[0].Value = Djlsh;
                par[1].Value = zhxmbhS;
                dt = SqlHepler.QueryTable(str.ToStr(), par);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    throw new Exception("未获取" + sm + "的个人项目信息");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("在获取" + sm + "个人项目时出现异常错误,错误源为:" + ex.ToStr());
            }
        }

        /// <summary>
        /// 获取卡号姓名
        /// </summary>
        /// <param name="tjbh"></param>
        /// <param name="tjcs"></param>
        /// <returns></returns>
        public string Getname(string gkhm)
        {
            string xm = "";
            string sm = string.Format("工卡号码:{0}", gkhm);
            try
            {
                string str = " select xm from futian_user.GZRY where gkhm=@gkhm";

                SqlParameter[] par =
                {
                    new SqlParameter("gkhm",SqlDbType.Char,6)
                };
                par[0].Value = gkhm;
                DataTable dt = new DataTable();
                dt = SqlHepler.QueryTable(str.ToStr(), par);
                if (dt.Rows.Count > 0)
                {
                    return dt.AsEnumerable().First()[0].ToStr();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("在获取" + sm + "姓名时出现异常错误,错误源为:" + ex.ToStr());
            }
        }

        /// <summary>
        /// 修改收费标记
        /// </summary>
        /// <param name="tjbh"></param>
        /// <param name="tjcs"></param>
        /// <returns></returns>
        public bool UpdCharged(string djlsh, string zhxmbh, string charged)
        {
            bool b = false;
            string sm = string.Format("登记流水号:{0},体检项目编号:{1}", djlsh, zhxmbh);
            try
            {
                string str = @"   update  a set charged=@charged from futian_user.tj_tjjlb a left join futian_user.tj_tjdjb b
                              on a.TJBH = b.TJBH and a.TJCS = b.TJCS where djlsh=@djlsh and a.tjxmbh=@tjxmbh";

                SqlParameter[] par =
                {
                    new SqlParameter("djlsh",SqlDbType.VarChar,12),
                    new SqlParameter("zhxmbh",SqlDbType.VarChar,6),
                    new SqlParameter("charged",SqlDbType.Char,1)
                };
                par[0].Value = djlsh.ToStr();
                par[1].Value = zhxmbh.ToStr();
                par[2].Value = charged.ToStr();

                int p = SqlHepler.ExecuteSql(str.ToStr(), par);
                if (p > 0)
                {
                    b = true;
                }
                else
                {
                    b = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(sm + "在修改收费标记时出现异常错误,错误源为:" + ex.Message.ToStr());
            }
            return b;
        }

        /// <summary>
        /// 修改登记表HISID
        /// </summary>
        /// <param name="tjbh"></param>
        /// <param name="tjcs"></param>
        /// <returns></returns>
        public bool UpdHisID(string djlsh, string HIS_PatientId, string HIS_ClinicNo)
        {
            bool b = false;
            string sm = string.Format("登记流水号:{0},his_patid:{1},his_blh:{2}", djlsh, HIS_PatientId, HIS_ClinicNo);
            try
            {
                string str = @"   update futian_user.tj_tjdjb set HIS_PatientId=@HIS_PatientId,HIS_ClinicNo=@HIS_ClinicNo where DJLSH=@DJLSH ";

                SqlParameter[] par =
                {
                    new SqlParameter("DJLSH",SqlDbType.VarChar,12),
                    new SqlParameter("HIS_PatientId",SqlDbType.VarChar,50),
                    new SqlParameter("HIS_ClinicNo",SqlDbType.VarChar,50)
                };
                par[0].Value = djlsh.ToStr();
                par[1].Value = HIS_PatientId.ToStr();
                par[2].Value = HIS_ClinicNo.ToStr();

                int p = SqlHepler.ExecuteSql(str.ToStr(), par);
                if (p > 0)
                {
                    b = true;
                }
                else
                {
                    b = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(sm + "在PatientID时出现异常错误,错误源为:" + ex.Message.ToStr());
            }
            return b;
        }

        /// <summary>
        /// 修改记录表处方序号和明细序号
        /// </summary>
        /// <param name="tjbh"></param>
        /// <param name="tjcs"></param>
        /// <returns></returns>
        public bool UpdHisCfxh(string djlsh, string zhxmbh, string his_cfxh, string his_cfmxxh)
        {
            bool b = false;
            string sm = string.Format("登记流水号:{0},组合项目编号:{1},处方序号:{2},处方明细序号:{3}", djlsh, zhxmbh, his_cfxh, his_cfmxxh);
            try
            {
                string str = @"   update a set a.his_cfxh=@his_cfxh,his_cfmxxh=@his_cfmxxh from futian_user.tj_tjjlb a 
                                  left join futian.user.tj_tjdjb b on a.tjbh= b.tjbh and a.tjcs=b.tjcs where b.DJLSH=@DJLSH and a.tjxmbh=@tjxmbh ";

                SqlParameter[] par =
                {
                    new SqlParameter("DJLSH",SqlDbType.VarChar,12),
                    new SqlParameter("tjxmbh",SqlDbType.Char,6),
                    new SqlParameter("his_cfxh",SqlDbType.VarChar,12),
                    new SqlParameter("his_cfmxxh",SqlDbType.VarChar,12)
                };
                par[0].Value = djlsh.ToStr();
                par[1].Value = zhxmbh.ToStr();
                par[2].Value = his_cfxh.ToStr();
                par[3].Value = his_cfmxxh.ToStr();

                int p = SqlHepler.ExecuteSql(str.ToStr(), par);
                if (p > 0)
                {
                    b = true;
                }
                else
                {
                    b = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(sm + "在修改his处方明细序号时出现异常错误,错误源为:" + ex.Message.ToStr());
            }
            return b;
        }

        /// <summary>
        /// 修改单位表HISID
        /// </summary>
        /// <param name="tjbh"></param>
        /// <param name="tjcs"></param>
        /// <returns></returns>
        public bool UpdDwHisID(string djlsh, string PatientID)
        {
            bool b = false;
            string sm = string.Format("登记流水号:{0},PatientID:{1}", djlsh, PatientID);
            try
            {
                string str = @"   update futian_user.hydwdmb set PatientID=@PatientID where bh=@dwbh ";

                SqlParameter[] par =
                {
                    new SqlParameter("dwbh",SqlDbType.VarChar,25),
                    new SqlParameter("PatientID",SqlDbType.VarChar,30)
                };
                par[0].Value = djlsh.ToStr();
                par[1].Value = PatientID.ToStr();

                int p = SqlHepler.ExecuteSql(str.ToStr(), par);
                if (p > 0)
                {
                    b = true;
                }
                else
                {
                    b = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(sm + "在PatientID时出现异常错误,错误源为:" + ex.Message.ToStr());
            }
            return b;
        }


        /// <summary>
        /// 获取套餐
        /// </summary>
        /// <param name="tjbh"></param>
        /// <param name="tjcs"></param>
        /// <returns></returns>
        public DataTable GetTc(string tjbh = "", int tjcs = 0, string Djlsh = "")
        {
            DataTable dt = new DataTable();
            string sm = string.Format("体检编号:{0},体检次数:{1},登记流水号:{2}", tjbh, tjcs, Djlsh);
            try
            {
                string str = " select A.TJXMBH,C.MC,B.DJLSH,c.jg from futian_user.TJ_TJJLB A LEFT JOIN futian_user.TJ_TJDJB B ON A.TJBH=B.TJBH AND A.TJCS=B.TJCS  LEFT JOIN futian_user.TJ_TC_HD C ON A.TJXMBH=C.BH WHERE LEN(A.TJXMBH)=2 ";
                if (string.IsNullOrEmpty(tjbh) || tjcs == 0)
                {
                    str += " AND Djlsh=@Djlsh";
                }
                else
                {
                    str += " and A.tjbh=@tjbh and A.tjcs=@tjcs";
                }
                SqlParameter[] par =
                 {
                    new SqlParameter("tjbh",SqlDbType.VarChar,14),
                    new SqlParameter("tjcs",SqlDbType.Int),
                    new SqlParameter("Djlsh",SqlDbType.Char,12)
                };
                par[0].Value = tjbh;
                par[1].Value = tjcs;
                par[2].Value = Djlsh;
                dt = SqlHepler.QueryTable(str.ToStr(), par);
            }
            catch (Exception ex)
            {
                throw new Exception("在获取" + sm + "个人套餐信息时出现异常错误,错误源为:" + ex.ToStr());
            }
            return dt;
        }


        /// <summary>
        /// 获取单位信息
        /// </summary>
        /// <param name="tjbh"></param>
        /// <param name="tjcs"></param>
        /// <returns></returns>
        public DataTable GetDwxx(string dwbh)
        {
            DataTable dt = new DataTable();
            string sm = string.Format("单位编号:{0}", dwbh);
            try
            {
                string str = " select PatientID,MC,ZHXGRQ,LXDH,LXDZ,DWFZR from futian_user.HYDWDMB where bh=@bh  ";

                SqlParameter[] par =
                 {
                    new SqlParameter("bh",SqlDbType.VarChar,25)
                };
                par[0].Value = dwbh;

                dt = SqlHepler.QueryTable(str.ToStr(), par);
            }
            catch (Exception ex)
            {
                throw new Exception("在获取" + sm + "单位信息时出现异常错误,错误源为:" + ex.ToStr());
            }
            return dt;
        }


        /// <summary>
        /// 查询Pacs结果表是否存在相同数据
        /// </summary>
        /// <param name="djlsh"></param>
        /// <param name="Zhxmbh"></param>
        /// <returns></returns>
        public bool SelPacs(string djlsh, string Zhxmbh)
        {
            bool b = false;
            try
            {

                StringBuilder cxlis = new StringBuilder();
                cxlis.Append(" select DJLSH from futian_user.TJ_PACSJGB where DJLSH=@DJLSH AND ZHXMBH=@ZHXMBH ");
                SqlParameter[] cxlispar =
                {
                    new SqlParameter("DJLSH",SqlDbType.Char,12),
                    new SqlParameter("ZHXMBH",SqlDbType.VarChar,100)
                };
                cxlispar[0].Value = djlsh;
                cxlispar[1].Value = Zhxmbh;
                SqlHepler Sql = new SqlHepler();
                DataTable lshdt = SqlHepler.QueryTable(cxlis.ToString(), cxlispar);
                if (lshdt.Rows.Count > 0)
                {
                    b = true;
                    return b;
                }
                else
                {
                    b = false;
                    return b;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("查询Pacs结果表", "500" + ":异常错误" + ex.Message);
            }
        }
        public bool SelSamePacs(PacsReport pacs)
        {
            bool b = false;
            try
            {

                StringBuilder cxlis = new StringBuilder();
                cxlis.Append(@" select DJLSH from futian_user.TJ_PACSJGB where DJLSH=@DJLSH AND ZHXMBH=@ZHXMBH  
                                and ZHXMMC=@ZHXMMC and JG=@JG and JCSJ=@JCSJ and JL=@JL and SHR=@SHR and SHRQ=@SHRQ 
                                and TPLJ=@TPLJ and CZY=@CZY  and JKID='2' and FLAG='0' and  JCQKFZSM=@JCQKFZSM  ");
                SqlParameter[] getxmpar = {
                new SqlParameter("@DJLSH", SqlDbType.VarChar, 14),
                new SqlParameter("@ZHXMBH", SqlDbType.VarChar, 64),
                new SqlParameter("@ZHXMMC", SqlDbType.VarChar, 255),
                new SqlParameter("@JG", SqlDbType.VarChar, 2000),
                new SqlParameter("@JCSJ", SqlDbType.VarChar, 2000),
                new SqlParameter("@JL", SqlDbType.VarChar, 2000),
                new SqlParameter("@SHR", SqlDbType.VarChar, 40),
                new SqlParameter("@SHRQ", SqlDbType.VarChar, 20),
                new SqlParameter("@TPLJ", SqlDbType.VarChar, 2000),
                new SqlParameter("@CZY", SqlDbType.VarChar, 40),
                new SqlParameter("@JCQKFZSM", SqlDbType.VarChar, 2000)
                };
                getxmpar[0].Value = pacs.djlsh;
                getxmpar[1].Value = pacs.zhxmbh;
                getxmpar[2].Value = pacs.zhxmmc;
                getxmpar[3].Value = pacs.jg;
                getxmpar[4].Value = pacs.jcsj;
                getxmpar[5].Value = pacs.jl;
                getxmpar[6].Value = pacs.shr;
                getxmpar[7].Value = pacs.shrq;
                getxmpar[8].Value = pacs.tplj;
                getxmpar[9].Value = pacs.czy;
                getxmpar[10].Value = pacs.jcqkfzsm;
                SqlHepler Sql = new SqlHepler();
                DataTable lshdt = SqlHepler.QueryTable(cxlis.ToString(), getxmpar);
                if (lshdt.Rows.Count > 0)
                {
                    b = true;
                    return b;
                }
                else
                {
                    b = false;
                    return b;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("查询Pacs结果表", "500" + ":异常错误" + ex.Message);
            }
        }

        /// <summary>
        /// 新增Pacs
        /// </summary>
        /// <param name="tj_entrt"></param>
        public int AddPacs(string djlsh, string zhxmbh, string zhxmmc, string jg, string jcsj, string jl, string shr, string shrq, string tplj, string czy, string jcqkfzsm)
        {
            DataTable xmdt = new DataTable();
            try
            {
                StringBuilder getxmsql = new StringBuilder(300, 2000);
                getxmsql.Append(" insert into futian_user.TJ_PACSJGB (DJLSH,ZHXMBH,ZHXMMC,JG,JCSJ,  ");
                getxmsql.Append(" JL,SHR,SHRQ,TPLJ,CZY,JKID,FLAG,JCQKFZSM) ");
                getxmsql.Append(" values(@DJLSH,@ZHXMBH,@ZHXMMC,@JG,@JCSJ,@JL");
                getxmsql.Append(" ,@SHR,@SHRQ,@TPLJ,@CZY,'2','0',@JCQKFZSM) ");

                SqlParameter[] getxmpar = {
                new SqlParameter("@DJLSH", SqlDbType.VarChar, 14),
                new SqlParameter("@ZHXMBH", SqlDbType.VarChar, 64),
                new SqlParameter("@ZHXMMC", SqlDbType.VarChar, 255),
                new SqlParameter("@JG", SqlDbType.VarChar, 2000),
                new SqlParameter("@JCSJ", SqlDbType.VarChar, 2000),
                new SqlParameter("@JL", SqlDbType.VarChar, 2000),
                new SqlParameter("@SHR", SqlDbType.VarChar, 40),
                new SqlParameter("@SHRQ", SqlDbType.VarChar, 20),
                new SqlParameter("@TPLJ", SqlDbType.VarChar, 2000),
                new SqlParameter("@CZY", SqlDbType.VarChar, 40),
                new SqlParameter("@JCQKFZSM", SqlDbType.VarChar, 2000)
               };
                getxmpar[0].Value = djlsh;
                getxmpar[1].Value = zhxmbh;
                getxmpar[2].Value = zhxmmc;
                getxmpar[3].Value = jg;
                getxmpar[4].Value = jcsj;
                getxmpar[5].Value = jl;
                getxmpar[6].Value = shr;
                getxmpar[7].Value = shrq;
                getxmpar[8].Value = tplj;
                getxmpar[9].Value = czy;
                getxmpar[10].Value = jcqkfzsm;

                SqlHepler Sql = new SqlHepler();
                int I = SqlHepler.ExecuteSql(getxmsql.ToString(), getxmpar);
                return I;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 修改Pacs
        /// </summary>
        /// <param name="tj_entrt"></param>
        public int UpdPacs(string djlsh, string zhxmbh, string zhxmmc, string jg, string jcsj, string jl, string shr, string shrq, string tplj, string czy, string jcqkfzsm)
        {
            DataTable xmdt = new DataTable();
            try
            {
                StringBuilder getxmsql = new StringBuilder(300, 2000);
                getxmsql.Append(" update futian_user.TJ_PACSJGB set ZHXMMC=@ZHXMMC,JG=@JG,JCSJ=@JCSJ,    ");
                getxmsql.Append(" JL=@JL,SHR=@SHR,SHRQ=@SHRQ,TPLJ=@TPLJ,CZY=@CZY ,JKID='2',FLAG='0', ");
                getxmsql.Append(" JCQKFZSM=@JCQKFZSM where DJLSH=@DJLSH and ZHXMBH=@ZHXMBH ");
                SqlParameter[] getxmpar = {
                new SqlParameter("@DJLSH", SqlDbType.VarChar, 14),
                new SqlParameter("@ZHXMBH", SqlDbType.VarChar, 64),
                new SqlParameter("@ZHXMMC", SqlDbType.VarChar, 255),
                new SqlParameter("@JG", SqlDbType.VarChar, 2000),
                new SqlParameter("@JCSJ", SqlDbType.VarChar, 2000),
                new SqlParameter("@JL", SqlDbType.VarChar, 2000),
                new SqlParameter("@SHR", SqlDbType.VarChar, 40),
                new SqlParameter("@SHRQ", SqlDbType.VarChar, 20),
                new SqlParameter("@TPLJ", SqlDbType.VarChar, 2000),
                new SqlParameter("@CZY", SqlDbType.VarChar, 40),
                new SqlParameter("@JCQKFZSM", SqlDbType.VarChar, 2000)
                };
                getxmpar[0].Value = djlsh;
                getxmpar[1].Value = zhxmbh;
                getxmpar[2].Value = zhxmmc;
                getxmpar[3].Value = jg;
                getxmpar[4].Value = jcsj;
                getxmpar[5].Value = jl;
                getxmpar[6].Value = shr;
                getxmpar[7].Value = shrq;
                getxmpar[8].Value = tplj;
                getxmpar[9].Value = czy;
                getxmpar[10].Value = jcqkfzsm;

                SqlHepler Sql = new SqlHepler();
                int I = SqlHepler.ExecuteSql(getxmsql.ToString(), getxmpar);
                return I;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 查询检验结果表是否存在相同数据
        /// </summary>
        /// <param name="djlsh"></param>
        /// <param name="Zhxmbh"></param>
        /// <param name="Xmbh"></param>
        /// <returns></returns>
        public bool SelLis(string djlsh, string Zhxmbh, string Xmbh)
        {
            bool b = false;
            try
            {

                StringBuilder cxlis = new StringBuilder();
                cxlis.Append(" select DJLSH from futian_user.TJ_JYJGB where DJLSH=@DJLSH AND ZHXMBH=@ZHXMBH AND XMBH=@XMBH");
                SqlParameter[] cxlispar =
                {
                    new SqlParameter("DJLSH",SqlDbType.Char,12),
                    new SqlParameter("XMBH",SqlDbType.VarChar,64),
                    new SqlParameter("ZHXMBH",SqlDbType.VarChar,6)
                };
                cxlispar[0].Value = djlsh;
                cxlispar[1].Value = Xmbh;
                cxlispar[2].Value = Zhxmbh;
                SqlHepler Sql = new SqlHepler();
                DataTable lshdt = SqlHepler.QueryTable(cxlis.ToString(), cxlispar);
                if (lshdt.Rows.Count > 0)
                {
                    b = true;
                    return b;
                }
                else
                {
                    b = false;
                    return b;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("查询LIS结果表", "500" + ":异常错误" + ex.Message);
            }
        }

        public bool SelAllLis(LisReport lis)
        {
            bool b = false;
            try
            {

                StringBuilder cxlis = new StringBuilder();
                cxlis.Append(@" select DJLSH from futian_user.TJ_JYJGB 
                        where DJLSH=@DJLSH AND ZHXMBH=@ZHXMBH AND XMBH=@XMBH and JG=@JG and CZY=@CZY 
                        and DW=@DW and ZHXMBH=@zhxmbh and ZHXMMC=@zhxmmc and ZHXMBH_LIS=@zhxmbh and ZHXMMC_LIS=@zhxmmc and CKFW=@CKFW and SHR=@SHR and SHRQ=@SHRQ and SYSDATETIME=@SYSDATETIME and PROMPT=@PROMPT and FLAG='0'");
                SqlParameter[] getxmpar =
                        {
                    new SqlParameter("DJLSH",SqlDbType.Char,12),
                    new SqlParameter("zhxmbh",SqlDbType.VarChar,6),
                    new SqlParameter("zhxmmc",SqlDbType.VarChar,32),
                    new SqlParameter("XMBH",SqlDbType.VarChar,64),
                    new SqlParameter("XMMC",SqlDbType.VarChar,255),
                    new SqlParameter("JG",SqlDbType.VarChar,255),
                    new SqlParameter("DW",SqlDbType.VarChar,50),
                    new SqlParameter("CKFW",SqlDbType.VarChar,200),
                    new SqlParameter("SHR",SqlDbType.VarChar,40),
                    new SqlParameter("SHRQ",SqlDbType.DateTime),
                    new SqlParameter("SYSDATETIME",SqlDbType.DateTime),
                    new SqlParameter("PROMPT",SqlDbType.VarChar,50),
                    new SqlParameter("CZY",SqlDbType.VarChar,40)
                };
                getxmpar[0].Value = lis.djlsh;
                getxmpar[1].Value = lis.zhxmbh;
                getxmpar[2].Value = lis.zhxmmc;
                getxmpar[3].Value = lis.xmbh;
                getxmpar[4].Value = lis.xmmc;
                getxmpar[5].Value = lis.jg;
                getxmpar[6].Value = lis.dw;
                getxmpar[7].Value = lis.ckfw;
                getxmpar[8].Value = lis.shr;
                getxmpar[9].Value = lis.shrq;
                getxmpar[10].Value = DateTime.Now.ToStr();
                getxmpar[11].Value = lis.zt;
                getxmpar[12].Value = lis.czy;
                SqlHepler Sql = new SqlHepler();
                DataTable lshdt = SqlHepler.QueryTable(cxlis.ToString(), getxmpar);
                if (lshdt.Rows.Count > 0)
                {
                    b = true;
                    return b;
                }
                else
                {
                    b = false;
                    return b;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("查询LIS结果表", "500" + ":异常错误" + ex.Message);
            }
        }

        /// <summary>
        /// 新增LIS
        /// </summary>
        /// <param name="tj_entrt"></param>
        public int AddLis(string djlsh, string zhxmbh, string zhxmmc, string xmbh, string xmmc, string jg, string dw, string ckfw, string shr, string shrq, string zt, string czy)
        {
            DataTable xmdt = new DataTable();
            try
            {
                StringBuilder getxmsql = new StringBuilder(300, 2000);
                getxmsql.Append(" INSERT INTO [futian_user].[TJ_JYJGB] ");
                getxmsql.Append(" ([DJLSH],[ZHXMBH],[ZHXMMC],[ZHXMBH_LIS],[ZHXMMC_LIS],[XMBH],[XMMC],[JG]");
                getxmsql.Append("  ,[DW],[CKFW],[SHR],[SHRQ],");
                getxmsql.Append(" [SYSDATETIME],[FLAG],[PROMPT],[CZY]) ");
                getxmsql.Append(" VALUES(@DJLSH,@zhxmbh,@zhxmmc,@zhxmbh,@zhxmmc,@XMBH,@XMMC,@JG, ");
                getxmsql.Append(" @DW,@CKFW,@SHR,@SHRQ,");
                getxmsql.Append(" @SYSDATETIME,0,@PROMPT,@CZY)");
                SqlParameter[] getxmpar =
                {
                    new SqlParameter("DJLSH",SqlDbType.Char,12),
                    new SqlParameter("zhxmbh",SqlDbType.VarChar,6),
                    new SqlParameter("zhxmmc",SqlDbType.VarChar,32),
                    new SqlParameter("XMBH",SqlDbType.VarChar,64),
                    new SqlParameter("XMMC",SqlDbType.VarChar,255),
                    new SqlParameter("JG",SqlDbType.VarChar,255),
                    new SqlParameter("DW",SqlDbType.VarChar,50),
                    new SqlParameter("CKFW",SqlDbType.VarChar,200),
                    new SqlParameter("SHR",SqlDbType.VarChar,40),
                    new SqlParameter("SHRQ",SqlDbType.DateTime),
                    new SqlParameter("SYSDATETIME",SqlDbType.DateTime),
                    new SqlParameter("PROMPT",SqlDbType.VarChar,50),
                    new SqlParameter("CZY",SqlDbType.VarChar,40)
                };
                getxmpar[0].Value = djlsh;
                getxmpar[1].Value = zhxmbh;
                getxmpar[2].Value = zhxmmc;
                getxmpar[3].Value = xmbh;
                getxmpar[4].Value = xmmc;
                getxmpar[5].Value = jg;
                getxmpar[6].Value = dw;
                getxmpar[7].Value = ckfw;
                getxmpar[8].Value = shr;
                getxmpar[9].Value = shrq;
                getxmpar[10].Value = DateTime.Now.ToStr();
                getxmpar[11].Value = zt;
                getxmpar[12].Value = czy;
                SqlHepler Sql = new SqlHepler();
                int I = SqlHepler.ExecuteSql(getxmsql.ToString(), getxmpar);
                return I;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 修改LIS
        /// </summary>
        /// <param name="tj_entrt"></param>
        public int UpdLis(string djlsh, string zhxmbh, string zhxmmc, string xmbh, string xmmc, string jg, string dw, string ckfw, string shr, string shrq, string zt, string czy)
        {
            DataTable xmdt = new DataTable();
            try
            {
                StringBuilder getxmsql = new StringBuilder(300, 2000);
                getxmsql.Append(" UPDATE futian_user.TJ_JYJGB SET JG=@JG,CZY=@CZY, ");
                getxmsql.Append(" DW=@DW,ZHXMBH=@zhxmbh,ZHXMMC=@zhxmmc,ZHXMBH_LIS=@zhxmbh,ZHXMMC_LIS=@zhxmmc,CKFW=@CKFW,SHR=@SHR,SHRQ=@SHRQ,SYSDATETIME=@SYSDATETIME,PROMPT=@PROMPT,FLAG='0'");
                getxmsql.Append(" WHERE DJLSH=@DJLSH AND XMBH=@XMBH ");
                SqlParameter[] getxmpar =
                {
                    new SqlParameter("DJLSH",SqlDbType.Char,12),
                    new SqlParameter("zhxmbh",SqlDbType.VarChar,6),
                    new SqlParameter("zhxmmc",SqlDbType.VarChar,32),
                    new SqlParameter("XMBH",SqlDbType.VarChar,64),
                    new SqlParameter("XMMC",SqlDbType.VarChar,255),
                    new SqlParameter("JG",SqlDbType.VarChar,255),
                    new SqlParameter("DW",SqlDbType.VarChar,50),
                    new SqlParameter("CKFW",SqlDbType.VarChar,200),
                    new SqlParameter("SHR",SqlDbType.VarChar,40),
                    new SqlParameter("SHRQ",SqlDbType.DateTime),
                    new SqlParameter("SYSDATETIME",SqlDbType.DateTime),
                    new SqlParameter("PROMPT",SqlDbType.VarChar,50),
                    new SqlParameter("CZY",SqlDbType.VarChar,40)
                };
                getxmpar[0].Value = djlsh;
                getxmpar[1].Value = zhxmbh;
                getxmpar[2].Value = zhxmmc;
                getxmpar[3].Value = xmbh;
                getxmpar[4].Value = xmmc;
                getxmpar[5].Value = jg;
                getxmpar[6].Value = dw;
                getxmpar[7].Value = ckfw;
                getxmpar[8].Value = shr;
                getxmpar[9].Value = shrq;
                getxmpar[10].Value = DateTime.Now.ToStr();
                getxmpar[11].Value = zt;
                getxmpar[12].Value = czy;
                SqlHepler Sql = new SqlHepler();
                int I = SqlHepler.ExecuteSql(getxmsql.ToString(), getxmpar);
                return I;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class LisReport
        {
            public string  djlsh { get; set; }
            public string zhxmbh { get; set; }
            public string zhxmmc { get; set; }
            public string xmbh { get; set; }
            public string xmmc { get; set; }
            public string jg { get; set; }
            public string dw { get; set; }
            public string ckfw { get; set; }
            public string shr { get; set; }
            public DateTime shrq { get; set; }
            public string zt { get; set; }
            public string czy { get; set; }

        }

        public class PacsReport
        {
            public string djlsh { get; set; }
            public string zhxmbh { get; set; }
            public string zhxmmc { get; set; }
            public string jg { get; set; }
            public string jcsj { get; set; }
            public string jl { get; set; }
            public string shr { get; set; }
            public DateTime shrq { get; set; }
            public string tplj { get; set; }
            public string czy { get; set; }
            public string jcqkfzsm { get; set; }

        }

        public Result UploadOnePacsReprot(PacsReport pacs,LogInfo log)
        {
            Result result = new Result();
            try
            {
                 if (SelPacs(pacs.djlsh, pacs.zhxmbh))
                {
                    if (!SelSamePacs(pacs))
                    {
                        if (UpdPacs(pacs.djlsh, pacs.zhxmbh, pacs.zhxmmc, pacs.jg, pacs.jcsj, pacs.jl, pacs.shr, pacs.shrq.ToString("yyyy/MM/dd HH:mm:ss"), pacs.tplj, pacs.czy, pacs.jcqkfzsm) > 0)
                        {
                            result.SetLog(200,"更新成功",log);
                        }else
                        {
                            result.SetLog(500,"更新失败",log);
                        }
                    }
                    else
                    {
                        result.SetLog(200, "此结果与数据库中保存的结果相同，无需进行保存，跳过", log);

                    }

                }else
                {
                    if (AddPacs(pacs.djlsh, pacs.zhxmbh, pacs.zhxmmc, pacs.jg, pacs.jcsj, pacs.jl, pacs.shr, pacs.shrq.ToString("yyyy/MM/dd HH:mm:ss"), pacs.tplj, pacs.czy, pacs.jcqkfzsm) > 0)
                    {
                        result.SetLog(200, "插入成功", log);
                    }else
                    {
                        result.SetLog(500, "插入失败", log);
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetLog(500, "上传pacs报告到pacsjgb时出现异常：" + ex.ExFormat(),log);
            }
            return result;
        }
        public Result UploadOneLisReprot(LisReport lis, LogInfo log)
        {
            Result result = new Result();
            try
            {
                if (SelLis(lis.djlsh, lis.zhxmbh,lis.xmbh))
                {
                    if (!SelAllLis(lis))
                    {
                        if (UpdLis(lis.djlsh,lis.zhxmbh,lis.zhxmmc,lis.xmbh,lis.xmmc,lis.jg,lis.dw,lis.ckfw,lis.shr,lis.shrq.ToString("yyyy/MM/dd HH:mm:ss"),lis.zt,lis.czy) > 0)
                        {
                            result.SetLog(200, "更新成功", log);
                        }
                        else
                        {
                            result.SetLog(500, "更新失败", log);
                        }
                    }
                    else
                    {
                        result.SetLog(200, "此结果与数据库中保存的结果相同，无需进行保存，跳过", log);

                    }

                }
                else
                {
                    if (AddLis(lis.djlsh, lis.zhxmbh, lis.zhxmmc, lis.xmbh, lis.xmmc, lis.jg, lis.dw, lis.ckfw, lis.shr, lis.shrq.ToString("yyyy/MM/dd HH:mm:ss"), lis.zt, lis.czy) > 0)
                    {
                        result.SetLog(200, "插入成功", log);
                    }
                    else
                    {
                        result.SetLog(500, "插入失败", log);
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetLog(500, "上传lis报告到jyjgb时出现异常：" + ex.ExFormat(), log);
            }
            return result;
        }

    }
}