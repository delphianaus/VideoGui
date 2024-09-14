using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.ffmpeg
{
    public enum HardwareAccelerator
    {
        /// <summary>
        ///     d3d11va
        /// </summary>
        d3d11va,

        /// <summary>
        ///     Automatically select the hardware acceleration method.
        /// </summary>
        auto,

        /// <summary>
        ///     Use DXVA2 (DirectX Video Acceleration) hardware acceleration.
        /// </summary>
        dxva2,

        /// <summary>
        ///     Use the Intel QuickSync Video acceleration for video transcoding.
        /// </summary>
        qsv,

        cuda,
        /// <summary>
        ///     cuvid
        /// </summary>
        cuvid,

        /// <summary>
        ///     Use VDPAU (Video Decode and Presentation API for Unix) hardware acceleration.
        /// </summary>
        vdpau,

        /// <summary>
        ///     Use VAAPI (Video Acceleration API) hardware acceleration.
        /// </summary>
        vaapi,

        vulkan,

        /// <summary>
        ///     
        /// </summary>
        libmfx,

        /// <summary>
        ///     
        /// </summary>
        software,
    }
    public enum AudioCodec
    {
        ///<summary>
        ///     4GV (Fourth Generation Vocoder)
        ///</summary>
        _4gv,

        ///<summary>
        ///     8SVX exponential
        ///</summary>
        _8svx_exp,

        ///<summary>
        ///     8SVX fibonacci
        ///</summary>
        _8svx_fib,

        ///<summary>
        ///     AAC (Advanced Audio Coding) (decoders: aac aac_fixed )
        ///</summary>
        aac,

        ///<summary>
        ///     AAC LATM (Advanced Audio Coding LATM syntax)
        ///</summary>
        aac_latm,

        ///<summary>
        ///     ATSC A/52A (AC-3) (decoders: ac3 ac3_fixed ) (encoders: ac3 ac3_fixed )
        ///</summary>
        ac3,

        ///<summary>
        ///     ADPCM 4X Movie
        ///</summary>
        adpcm_4xm,

        ///<summary>
        ///     SEGA CRI ADX ADPCM
        ///</summary>
        adpcm_adx,

        ///<summary>
        ///     ADPCM Nintendo Gamecube AFC
        ///</summary>
        adpcm_afc,

        ///<summary>
        ///     ADPCM AmuseGraphics Movie AGM
        ///</summary>
        adpcm_agm,

        ///<summary>
        ///     ADPCM Yamaha AICA
        ///</summary>
        adpcm_aica,

        ///<summary>
        ///     ADPCM Creative Technology
        ///</summary>
        adpcm_ct,

        ///<summary>
        ///     ADPCM Nintendo Gamecube DTK
        ///</summary>
        adpcm_dtk,

        ///<summary>
        ///     ADPCM Electronic Arts
        ///</summary>
        adpcm_ea,

        ///<summary>
        ///     ADPCM Electronic Arts Maxis CDROM XA
        ///</summary>
        adpcm_ea_maxis_xa,

        ///<summary>
        ///     ADPCM Electronic Arts R1
        ///</summary>
        adpcm_ea_r1,

        ///<summary>
        ///     ADPCM Electronic Arts R2
        ///</summary>
        adpcm_ea_r2,

        ///<summary>
        ///     ADPCM Electronic Arts R3
        ///</summary>
        adpcm_ea_r3,

        ///<summary>
        ///     ADPCM Electronic Arts XAS
        ///</summary>
        adpcm_ea_xas,

        ///<summary>
        ///     G.722 ADPCM (decoders: g722 ) (encoders: g722 )
        ///</summary>
        adpcm_g722,

        ///<summary>
        ///     G.726 ADPCM (decoders: g726 ) (encoders: g726 )
        ///</summary>
        adpcm_g726,

        ///<summary>
        ///     G.726 ADPCM little-endian (decoders: g726le ) (encoders: g726le )
        ///</summary>
        adpcm_g726le,

        ///<summary>
        ///     ADPCM IMA AMV
        ///</summary>
        adpcm_ima_amv,

        ///<summary>
        ///     ADPCM IMA CRYO APC
        ///</summary>
        adpcm_ima_apc,

        ///<summary>
        ///     ADPCM IMA Eurocom DAT4
        ///</summary>
        adpcm_ima_dat4,

        ///<summary>
        ///     ADPCM IMA Duck DK3
        ///</summary>
        adpcm_ima_dk3,

        ///<summary>
        ///     ADPCM IMA Duck DK4
        ///</summary>
        adpcm_ima_dk4,

        ///<summary>
        ///     ADPCM IMA Electronic Arts EACS
        ///</summary>
        adpcm_ima_ea_eacs,

        ///<summary>
        ///     ADPCM IMA Electronic Arts SEAD
        ///</summary>
        adpcm_ima_ea_sead,

        ///<summary>
        ///     ADPCM IMA Funcom ISS
        ///</summary>
        adpcm_ima_iss,

        ///<summary>
        ///     ADPCM IMA Dialogic OKI
        ///</summary>
        adpcm_ima_oki,

        ///<summary>
        ///     ADPCM IMA QuickTime
        ///</summary>
        adpcm_ima_qt,

        ///<summary>
        ///     ADPCM IMA Radical
        ///</summary>
        adpcm_ima_rad,

        ///<summary>
        ///     ADPCM IMA Loki SDL MJPEG
        ///</summary>
        adpcm_ima_smjpeg,

        ///<summary>
        ///     ADPCM IMA WAV
        ///</summary>
        adpcm_ima_wav,

        ///<summary>
        ///     ADPCM IMA Westwood
        ///</summary>
        adpcm_ima_ws,

        ///<summary>
        ///     ADPCM Microsoft
        ///</summary>
        adpcm_ms,

        ///<summary>
        ///     ADPCM MTAF
        ///</summary>
        adpcm_mtaf,

        ///<summary>
        ///     ADPCM Playstation
        ///</summary>
        adpcm_psx,

        ///<summary>
        ///     ADPCM Sound Blaster Pro 2-bit
        ///</summary>
        adpcm_sbpro_2,

        ///<summary>
        ///     ADPCM Sound Blaster Pro 2.6-bit
        ///</summary>
        adpcm_sbpro_3,

        ///<summary>
        ///     ADPCM Sound Blaster Pro 4-bit
        ///</summary>
        adpcm_sbpro_4,

        ///<summary>
        ///     ADPCM Shockwave Flash
        ///</summary>
        adpcm_swf,

        ///<summary>
        ///     ADPCM Nintendo THP
        ///</summary>
        adpcm_thp,

        ///<summary>
        ///     ADPCM Nintendo THP (Little-Endian)
        ///</summary>
        adpcm_thp_le,

        ///<summary>
        ///     LucasArts VIMA audio
        ///</summary>
        adpcm_vima,

        ///<summary>
        ///     ADPCM CDROM XA
        ///</summary>
        adpcm_xa,

        ///<summary>
        ///     ADPCM Yamaha
        ///</summary>
        adpcm_yamaha,

        ///<summary>
        ///     ALAC (Apple Lossless Audio Codec)
        ///</summary>
        alac,

        ///<summary>
        ///     AMR-NB (Adaptive Multi-Rate NarrowBand) (decoders: amrnb libopencore_amrnb ) (encoders: libopencore_amrnb )
        ///</summary>
        amr_nb,

        ///<summary>
        ///     AMR-WB (Adaptive Multi-Rate WideBand) (decoders: amrwb libopencore_amrwb ) (encoders: libvo_amrwbenc )
        ///</summary>
        amr_wb,

        ///<summary>
        ///     Monkey's Audio
        ///</summary>
        ape,

        ///<summary>
        ///     aptX (Audio Processing Technology for Bluetooth)
        ///</summary>
        aptx,

        ///<summary>
        ///     aptX HD (Audio Processing Technology for Bluetooth)
        ///</summary>
        aptx_hd,

        ///<summary>
        ///     ATRAC1 (Adaptive TRansform Acoustic Coding)
        ///</summary>
        atrac1,

        ///<summary>
        ///     ATRAC3 (Adaptive TRansform Acoustic Coding 3)
        ///</summary>
        atrac3,

        ///<summary>
        ///     ATRAC3 AL (Adaptive TRansform Acoustic Coding 3 Advanced Lossless)
        ///</summary>
        atrac3al,

        ///<summary>
        ///     ATRAC3+ (Adaptive TRansform Acoustic Coding 3+) (decoders: atrac3plus )
        ///</summary>
        atrac3p,

        ///<summary>
        ///     ATRAC3+ AL (Adaptive TRansform Acoustic Coding 3+ Advanced Lossless) (decoders: atrac3plusal )
        ///</summary>
        atrac3pal,

        ///<summary>
        ///     ATRAC9 (Adaptive TRansform Acoustic Coding 9)
        ///</summary>
        atrac9,

        ///<summary>
        ///     On2 Audio for Video Codec (decoders: on2avc )
        ///</summary>
        avc,

        ///<summary>
        ///     Bink Audio (DCT)
        ///</summary>
        binkaudio_dct,

        ///<summary>
        ///     Bink Audio (RDFT)
        ///</summary>
        binkaudio_rdft,

        ///<summary>
        ///     Discworld II BMV audio
        ///</summary>
        bmv_audio,

        ///<summary>
        ///     Constrained Energy Lapped Transform (CELT)
        ///</summary>
        celt,

        ///<summary>
        ///     codec2 (very low bitrate speech codec)
        ///</summary>
        codec2,

        ///<summary>
        ///     RFC 3389 Comfort Noise
        ///</summary>
        comfortnoise,

        ///<summary>
        ///     Cook / Cooker / Gecko (RealAudio G2)
        ///</summary>
        cook,

        ///<summary>
        ///     Dolby E
        ///</summary>
        dolby_e,

        ///<summary>
        ///     DSD (Direct Stream Digital), least significant bit first
        ///</summary>
        dsd_lsbf,

        ///<summary>
        ///     DSD (Direct Stream Digital), least significant bit first, planar
        ///</summary>
        dsd_lsbf_planar,

        ///<summary>
        ///     DSD (Direct Stream Digital), most significant bit first
        ///</summary>
        dsd_msbf,

        ///<summary>
        ///     DSD (Direct Stream Digital), most significant bit first, planar
        ///</summary>
        dsd_msbf_planar,

        ///<summary>
        ///     Delphine Software International CIN audio
        ///</summary>
        dsicinaudio,

        ///<summary>
        ///     Digital Speech Standard - Standard Play mode (DSS SP)
        ///</summary>
        dss_sp,

        ///<summary>
        ///     DST (Direct Stream Transfer)
        ///</summary>
        dst,

        ///<summary>
        ///     DCA (DTS Coherent Acoustics) (decoders: dca ) (encoders: dca )
        ///</summary>
        dts,

        ///<summary>
        ///     DV audio
        ///</summary>
        dvaudio,

        ///<summary>
        ///     ATSC A/52B (AC-3, E-AC-3)
        ///</summary>
        eac3,

        ///<summary>
        ///     EVRC (Enhanced Variable Rate Codec)
        ///</summary>
        evrc,

        ///<summary>
        ///     FLAC (Free Lossless Audio Codec)
        ///</summary>
        flac,

        ///<summary>
        ///     G.723.1
        ///</summary>
        g723_1,

        ///<summary>
        ///     G.729
        ///</summary>
        g729,

        ///<summary>
        ///     DPCM Gremlin
        ///</summary>
        gremlin_dpcm,

        ///<summary>
        ///     GSM
        ///</summary>
        gsm,

        ///<summary>
        ///     GSM Microsoft variant
        ///</summary>
        gsm_ms,

        ///<summary>
        ///     HCOM Audio
        ///</summary>
        hcom,

        ///<summary>
        ///     IAC (Indeo Audio Coder)
        ///</summary>
        iac,

        ///<summary>
        ///     iLBC (Internet Low Bitrate Codec)
        ///</summary>
        ilbc,

        ///<summary>
        ///     IMC (Intel Music Coder)
        ///</summary>
        imc,

        ///<summary>
        ///     DPCM Interplay
        ///</summary>
        interplay_dpcm,

        ///<summary>
        ///     Interplay ACM
        ///</summary>
        interplayacm,

        ///<summary>
        ///     MACE (Macintosh Audio Compression/Expansion) 3:1
        ///</summary>
        mace3,

        ///<summary>
        ///     MACE (Macintosh Audio Compression/Expansion) 6:1
        ///</summary>
        mace6,

        ///<summary>
        ///     Voxware MetaSound
        ///</summary>
        metasound,

        ///<summary>
        ///     MLP (Meridian Lossless Packing)
        ///</summary>
        mlp,

        ///<summary>
        ///     MP1 (MPEG audio layer 1) (decoders: mp1 mp1float )
        ///</summary>
        mp1,

        ///<summary>
        ///     MP2 (MPEG audio layer 2) (decoders: mp2 mp2float ) (encoders: mp2 mp2fixed libtwolame )
        ///</summary>
        mp2,

        ///<summary>
        ///     MP3 (MPEG audio layer 3) (decoders: mp3float mp3 ) (encoders: libmp3lame libshine )
        ///</summary>
        mp3,

        ///<summary>
        ///     ADU (Application Data Unit) MP3 (MPEG audio layer 3) (decoders: mp3adufloat mp3adu )
        ///</summary>
        mp3adu,

        ///<summary>
        ///     MP3onMP4 (decoders: mp3on4float mp3on4 )
        ///</summary>
        mp3on4,

        ///<summary>
        ///     MPEG-4 Audio Lossless Coding (ALS) (decoders: als )
        ///</summary>
        mp4als,

        ///<summary>
        ///     Musepack SV7 (decoders: mpc7 )
        ///</summary>
        musepack7,

        ///<summary>
        ///     Musepack SV8 (decoders: mpc8 )
        ///</summary>
        musepack8,

        ///<summary>
        ///     Nellymoser Asao
        ///</summary>
        nellymoser,

        ///<summary>
        ///     Opus (Opus Interactive Audio Codec) (decoders: opus libopus ) (encoders: opus libopus )
        ///</summary>
        opus,

        ///<summary>
        ///     Amazing Studio Packed Animation File Audio
        ///</summary>
        paf_audio,

        ///<summary>
        ///     PCM A-law / G.711 A-law
        ///</summary>
        pcm_alaw,

        ///<summary>
        ///     PCM signed 16|20|24-bit big-endian for Blu-ray media
        ///</summary>
        pcm_bluray,

        ///<summary>
        ///     PCM signed 20|24-bit big-endian
        ///</summary>
        pcm_dvd,

        ///<summary>
        ///     PCM 16.8 floating point little-endian
        ///</summary>
        pcm_f16le,

        ///<summary>
        ///     PCM 24.0 floating point little-endian
        ///</summary>
        pcm_f24le,

        ///<summary>
        ///     PCM 32-bit floating point big-endian
        ///</summary>
        pcm_f32be,

        ///<summary>
        ///     PCM 32-bit floating point little-endian
        ///</summary>
        pcm_f32le,

        ///<summary>
        ///     PCM 64-bit floating point big-endian
        ///</summary>
        pcm_f64be,

        ///<summary>
        ///     PCM 64-bit floating point little-endian
        ///</summary>
        pcm_f64le,

        ///<summary>
        ///     PCM signed 20-bit little-endian planar
        ///</summary>
        pcm_lxf,

        ///<summary>
        ///     PCM mu-law / G.711 mu-law
        ///</summary>
        pcm_mulaw,

        ///<summary>
        ///     PCM signed 16-bit big-endian
        ///</summary>
        pcm_s16be,

        ///<summary>
        ///     PCM signed 16-bit big-endian planar
        ///</summary>
        pcm_s16be_planar,

        ///<summary>
        ///     PCM signed 16-bit little-endian
        ///</summary>
        pcm_s16le,

        ///<summary>
        ///     PCM signed 16-bit little-endian planar
        ///</summary>
        pcm_s16le_planar,

        ///<summary>
        ///     PCM signed 24-bit big-endian
        ///</summary>
        pcm_s24be,

        ///<summary>
        ///     PCM D-Cinema audio signed 24-bit
        ///</summary>
        pcm_s24daud,

        ///<summary>
        ///     PCM signed 24-bit little-endian
        ///</summary>
        pcm_s24le,

        ///<summary>
        ///     PCM signed 24-bit little-endian planar
        ///</summary>
        pcm_s24le_planar,

        ///<summary>
        ///     PCM signed 32-bit big-endian
        ///</summary>
        pcm_s32be,

        ///<summary>
        ///     PCM signed 32-bit little-endian
        ///</summary>
        pcm_s32le,

        ///<summary>
        ///     PCM signed 32-bit little-endian planar
        ///</summary>
        pcm_s32le_planar,

        ///<summary>
        ///     PCM signed 64-bit big-endian
        ///</summary>
        pcm_s64be,

        ///<summary>
        ///     PCM signed 64-bit little-endian
        ///</summary>
        pcm_s64le,

        ///<summary>
        ///     PCM signed 8-bit
        ///</summary>
        pcm_s8,

        ///<summary>
        ///     PCM signed 8-bit planar
        ///</summary>
        pcm_s8_planar,

        ///<summary>
        ///     PCM unsigned 16-bit big-endian
        ///</summary>
        pcm_u16be,

        ///<summary>
        ///     PCM unsigned 16-bit little-endian
        ///</summary>
        pcm_u16le,

        ///<summary>
        ///     PCM unsigned 24-bit big-endian
        ///</summary>
        pcm_u24be,

        ///<summary>
        ///     PCM unsigned 24-bit little-endian
        ///</summary>
        pcm_u24le,

        ///<summary>
        ///     PCM unsigned 32-bit big-endian
        ///</summary>
        pcm_u32be,

        ///<summary>
        ///     PCM unsigned 32-bit little-endian
        ///</summary>
        pcm_u32le,

        ///<summary>
        ///     PCM unsigned 8-bit
        ///</summary>
        pcm_u8,

        ///<summary>
        ///     PCM Archimedes VIDC
        ///</summary>
        pcm_vidc,

        ///<summary>
        ///     PCM Zork
        ///</summary>
        pcm_zork,

        ///<summary>
        ///     QCELP / PureVoice
        ///</summary>
        qcelp,

        ///<summary>
        ///     QDesign Music Codec 2
        ///</summary>
        qdm2,

        ///<summary>
        ///     QDesign Music
        ///</summary>
        qdmc,

        ///<summary>
        ///     RealAudio 1.0 (14.4K) (decoders: real_144 ) (encoders: real_144 )
        ///</summary>
        ra_144,

        ///<summary>
        ///     RealAudio 2.0 (28.8K) (decoders: real_288 )
        ///</summary>
        ra_288,

        ///<summary>
        ///     RealAudio Lossless
        ///</summary>
        ralf,

        ///<summary>
        ///     DPCM id RoQ
        ///</summary>
        roq_dpcm,

        ///<summary>
        ///     SMPTE 302M
        ///</summary>
        s302m,

        ///<summary>
        ///     SBC (low-complexity subband codec)
        ///</summary>
        sbc,

        ///<summary>
        ///     DPCM Squareroot-Delta-Exact
        ///</summary>
        sdx2_dpcm,

        ///<summary>
        ///     Shorten
        ///</summary>
        shorten,

        ///<summary>
        ///     RealAudio SIPR / ACELP.NET
        ///</summary>
        sipr,

        ///<summary>
        ///     Smacker audio (decoders: smackaud )
        ///</summary>
        smackaudio,

        ///<summary>
        ///     SMV (Selectable Mode Vocoder)
        ///</summary>
        smv,

        ///<summary>
        ///     DPCM Sol
        ///</summary>
        sol_dpcm,

        ///<summary>
        ///     Sonic
        ///</summary>
        sonic,

        ///<summary>
        ///     Sonic lossless
        ///</summary>
        sonicls,

        ///<summary>
        ///     Speex (decoders: libspeex ) (encoders: libspeex )
        ///</summary>
        speex,

        ///<summary>
        ///     TAK (Tom's lossless Audio Kompressor)
        ///</summary>
        tak,

        ///<summary>
        ///     TrueHD
        ///</summary>
        truehd,

        ///<summary>
        ///     DSP Group TrueSpeech
        ///</summary>
        truespeech,

        ///<summary>
        ///     TTA (True Audio)
        ///</summary>
        tta,

        ///<summary>
        ///     VQF TwinVQ
        ///</summary>
        twinvq,

        ///<summary>
        ///     Sierra VMD audio
        ///</summary>
        vmdaudio,

        ///<summary>
        ///     Vorbis (decoders: vorbis libvorbis ) (encoders: vorbis libvorbis )
        ///</summary>
        vorbis,

        ///<summary>
        ///     Wave synthesis pseudo-codec
        ///</summary>
        wavesynth,

        ///<summary>
        ///     WavPack (encoders: wavpack libwavpack )
        ///</summary>
        wavpack,

        ///<summary>
        ///     Westwood Audio (SND1) (decoders: ws_snd1 )
        ///</summary>
        westwood_snd1,

        ///<summary>
        ///     Windows Media Audio Lossless
        ///</summary>
        wmalossless,

        ///<summary>
        ///     Windows Media Audio 9 Professional
        ///</summary>
        wmapro,

        ///<summary>
        ///     Windows Media Audio 1
        ///</summary>
        wmav1,

        ///<summary>
        ///     Windows Media Audio 2
        ///</summary>
        wmav2,

        ///<summary>
        ///     Windows Media Audio Voice
        ///</summary>
        wmavoice,

        ///<summary>
        ///     DPCM Xan
        ///</summary>
        xan_dpcm,

        ///<summary>
        ///     Xbox Media Audio 1
        ///</summary>
        xma1,

        ///<summary>
        ///     Xbox Media Audio 2
        ///</summary>
        xma2,

        ///<summary>
        ///     libvorbis
        ///</summary>
        libvorbis,

        ///<summary>
        ///     copy
        ///</summary>
        copy,

        ///<summary>
        ///     Opus (Opus Interactive Audio Codec) (decoders: opus libopus ) (encoders: opus libopus )
        ///</summary>
        libopus
    }
    public enum Scaling
    {
        point,
        bilinear,
        bicubic,
        spline16,
        spline36,
        lanczos
    }
    public enum StreamType
    {
        /// <summary>
        ///     Video stream
        /// </summary>
        Video = 0,

        /// <summary>
        ///     Audio stream
        /// </summary>
        Audio = 1,

        /// <summary>
        ///     Subtitle stream
        /// </summary>
        Subtitle = 2
    }
    public enum Format
    {
        ///<summary>
        ///     3DO STR
        ///</summary>
        _3dostr,

        ///<summary>
        ///     3GP2 (3GPP2 file format)
        ///</summary>
        _3g2,

        ///<summary>
        ///     3GP (3GPP file format)
        ///</summary>
        _3gp,

        ///<summary>
        ///     4X Technologies
        ///</summary>
        _4xm,

        ///<summary>
        ///     a64 - video for Commodore 64
        ///</summary>
        a64,

        ///<summary>
        ///     Audible AA format files
        ///</summary>
        aa,

        ///<summary>
        ///     raw ADTS AAC (Advanced Audio Coding)
        ///</summary>
        aac,

        ///<summary>
        ///     raw AC-3
        ///</summary>
        ac3,

        ///<summary>
        ///     Interplay ACM
        ///</summary>
        acm,

        ///<summary>
        ///     ACT Voice file format
        ///</summary>
        act,

        ///<summary>
        ///     Artworx Data Format
        ///</summary>
        adf,

        ///<summary>
        ///     ADP
        ///</summary>
        adp,

        ///<summary>
        ///     Sony PS2 ADS
        ///</summary>
        ads,

        ///<summary>
        ///     ADTS AAC (Advanced Audio Coding)
        ///</summary>
        adts,

        ///<summary>
        ///     CRI ADX
        ///</summary>
        adx,

        ///<summary>
        ///     MD STUDIO audio
        ///</summary>
        aea,

        ///<summary>
        ///     AFC
        ///</summary>
        afc,

        ///<summary>
        ///     Audio IFF
        ///</summary>
        aiff,

        ///<summary>
        ///     CRI AIX
        ///</summary>
        aix,

        ///<summary>
        ///     PCM A-law
        ///</summary>
        alaw,

        ///<summary>
        ///     Alias/Wavefront PIX image
        ///</summary>
        alias_pix,

        ///<summary>
        ///     3GPP AMR
        ///</summary>
        amr,

        ///<summary>
        ///     raw AMR-NB
        ///</summary>
        amrnb,

        ///<summary>
        ///     raw AMR-WB
        ///</summary>
        amrwb,

        ///<summary>
        ///     Deluxe Paint Animation
        ///</summary>
        anm,

        ///<summary>
        ///     CRYO APC
        ///</summary>
        apc,

        ///<summary>
        ///     Monkey's Audio
        ///</summary>
        ape,

        ///<summary>
        ///     Animated Portable Network Graphics
        ///</summary>
        apng,

        ///<summary>
        ///     raw aptX (Audio Processing Technology for Bluetooth)
        ///</summary>
        aptx,

        ///<summary>
        ///     raw aptX HD (Audio Processing Technology for Bluetooth)
        ///</summary>
        aptx_hd,

        ///<summary>
        ///     AQTitle subtitles
        ///</summary>
        aqtitle,

        ///<summary>
        ///     ASF (Advanced / Active Streaming Format)
        ///</summary>
        asf,

        ///<summary>
        ///     ASF (Advanced / Active Streaming Format)
        ///</summary>
        asf_o,

        ///<summary>
        ///     ASF (Advanced / Active Streaming Format)
        ///</summary>
        asf_stream,

        ///<summary>
        ///     SSA (SubStation Alpha) subtitle
        ///</summary>
        ass,

        ///<summary>
        ///     AST (Audio Stream)
        ///</summary>
        ast,

        ///<summary>
        ///     Sun AU
        ///</summary>
        au,

        ///<summary>
        ///     AVI (Audio Video Interleaved)
        ///</summary>
        avi,

        ///<summary>
        ///     AviSynth script
        ///</summary>
        avisynth,

        ///<summary>
        ///     SWF (ShockWave Flash) (AVM2)
        ///</summary>
        avm2,

        ///<summary>
        ///     AVR (Audio Visual Research)
        ///</summary>
        avr,

        ///<summary>
        ///     Argonaut Games Creature Shock
        ///</summary>
        avs,

        ///<summary>
        ///     raw AVS2-P2/IEEE1857.4 video
        ///</summary>
        avs2,

        ///<summary>
        ///     Bethesda Softworks VID
        ///</summary>
        bethsoftvid,

        ///<summary>
        ///     Brute Force and Ignorance
        ///</summary>
        bfi,

        ///<summary>
        ///     BFSTM (Binary Cafe Stream)
        ///</summary>
        bfstm,

        ///<summary>
        ///     Binary text
        ///</summary>
        bin,

        ///<summary>
        ///     Bink
        ///</summary>
        bink,

        ///<summary>
        ///     G.729 BIT file format
        ///</summary>
        bit,

        ///<summary>
        ///     piped bmp sequence
        ///</summary>
        bmp_pipe,

        ///<summary>
        ///     Discworld II BMV
        ///</summary>
        bmv,

        ///<summary>
        ///     Black Ops Audio
        ///</summary>
        boa,

        ///<summary>
        ///     BRender PIX image
        ///</summary>
        brender_pix,

        ///<summary>
        ///     BRSTM (Binary Revolution Stream)
        ///</summary>
        brstm,

        ///<summary>
        ///     Interplay C93
        ///</summary>
        c93,

        ///<summary>
        ///     Apple CAF (Core Audio Format)
        ///</summary>
        caf,

        ///<summary>
        ///     raw Chinese AVS (Audio Video Standard) video
        ///</summary>
        cavsvideo,

        ///<summary>
        ///     CD Graphics
        ///</summary>
        cdg,

        ///<summary>
        ///     Commodore CDXL video
        ///</summary>
        cdxl,

        ///<summary>
        ///     Phantom Cine
        ///</summary>
        cine,

        ///<summary>
        ///     codec2 .c2 muxer
        ///</summary>
        codec2,

        ///<summary>
        ///     raw codec2 muxer
        ///</summary>
        codec2raw,

        ///<summary>
        ///     Virtual concatenation script
        ///</summary>
        concat,

        ///<summary>
        ///     CRC testing
        ///</summary>
        crc,

        ///<summary>
        ///     DASH Muxer
        ///</summary>
        dash,

        ///<summary>
        ///     raw data
        ///</summary>
        data,

        ///<summary>
        ///     D-Cinema audio
        ///</summary>
        daud,

        ///<summary>
        ///     Sega DC STR
        ///</summary>
        dcstr,

        ///<summary>
        ///     piped dds sequence
        ///</summary>
        dds_pipe,

        ///<summary>
        ///     Chronomaster DFA
        ///</summary>
        dfa,

        ///<summary>
        ///     Video DAV
        ///</summary>
        dhav,

        ///<summary>
        ///     raw Dirac
        ///</summary>
        dirac,

        ///<summary>
        ///     raw DNxHD (SMPTE VC-3)
        ///</summary>
        dnxhd,

        ///<summary>
        ///     piped dpx sequence
        ///</summary>
        dpx_pipe,

        ///<summary>
        ///     DSD Stream File (DSF)
        ///</summary>
        dsf,

        ///<summary>
        ///     DirectShow capture
        ///</summary>
        dshow,

        ///<summary>
        ///     Delphine Software International CIN
        ///</summary>
        dsicin,

        ///<summary>
        ///     Digital Speech Standard (DSS)
        ///</summary>
        dss,

        ///<summary>
        ///     raw DTS
        ///</summary>
        dts,

        ///<summary>
        ///     raw DTS-HD
        ///</summary>
        dtshd,

        ///<summary>
        ///     DV (Digital Video)
        ///</summary>
        dv,

        ///<summary>
        ///     raw dvbsub
        ///</summary>
        dvbsub,

        ///<summary>
        ///     dvbtxt
        ///</summary>
        dvbtxt,

        ///<summary>
        ///     MPEG-2 PS (DVD VOB)
        ///</summary>
        dvd,

        ///<summary>
        ///     DXA
        ///</summary>
        dxa,

        ///<summary>
        ///     Electronic Arts Multimedia
        ///</summary>
        ea,

        ///<summary>
        ///     Electronic Arts cdata
        ///</summary>
        ea_cdata,

        ///<summary>
        ///     raw E-AC-3
        ///</summary>
        eac3,

        ///<summary>
        ///     Ensoniq Paris Audio File
        ///</summary>
        epaf,

        ///<summary>
        ///     piped exr sequence
        ///</summary>
        exr_pipe,

        ///<summary>
        ///     PCM 32-bit floating-point big-endian
        ///</summary>
        f32be,

        ///<summary>
        ///     PCM 32-bit floating-point little-endian
        ///</summary>
        f32le,

        ///<summary>
        ///     F4V Adobe Flash Video
        ///</summary>
        f4v,

        ///<summary>
        ///     PCM 64-bit floating-point big-endian
        ///</summary>
        f64be,

        ///<summary>
        ///     PCM 64-bit floating-point little-endian
        ///</summary>
        f64le,

        ///<summary>
        ///     FFmpeg metadata in text
        ///</summary>
        ffmetadata,

        ///<summary>
        ///     FIFO queue pseudo-muxer
        ///</summary>
        fifo,

        ///<summary>
        ///     Fifo test muxer
        ///</summary>
        fifo_test,

        ///<summary>
        ///     Sega FILM / CPK
        ///</summary>
        film_cpk,

        ///<summary>
        ///     Adobe Filmstrip
        ///</summary>
        filmstrip,

        ///<summary>
        ///     Flexible Image Transport System
        ///</summary>
        fits,

        ///<summary>
        ///     raw FLAC
        ///</summary>
        flac,

        ///<summary>
        ///     FLI/FLC/FLX animation
        ///</summary>
        flic,

        ///<summary>
        ///     FLV (Flash Video)
        ///</summary>
        flv,

        ///<summary>
        ///     framecrc testing
        ///</summary>
        framecrc,

        ///<summary>
        ///     Per-frame hash testing
        ///</summary>
        framehash,

        ///<summary>
        ///     Per-frame MD5 testing
        ///</summary>
        framemd5,

        ///<summary>
        ///     Megalux Frame
        ///</summary>
        frm,

        ///<summary>
        ///     FMOD Sample Bank
        ///</summary>
        fsb,

        ///<summary>
        ///     raw G.722
        ///</summary>
        g722,

        ///<summary>
        ///     raw G.723.1
        ///</summary>
        g723_1,

        ///<summary>
        ///     raw big-endian G.726 ("left-justified")
        ///</summary>
        g726,

        ///<summary>
        ///     raw little-endian G.726 ("right-justified")
        ///</summary>
        g726le,

        ///<summary>
        ///     G.729 raw format demuxer
        ///</summary>
        g729,

        ///<summary>
        ///     GDI API Windows frame grabber
        ///</summary>
        gdigrab,

        ///<summary>
        ///     Gremlin Digital Video
        ///</summary>
        gdv,

        ///<summary>
        ///     GENeric Header
        ///</summary>
        genh,

        ///<summary>
        ///     CompuServe Graphics Interchange Format (GIF)
        ///</summary>
        gif,

        ///<summary>
        ///     piped gif sequence
        ///</summary>
        gif_pipe,

        ///<summary>
        ///     raw GSM
        ///</summary>
        gsm,

        ///<summary>
        ///     GXF (General eXchange Format)
        ///</summary>
        gxf,

        ///<summary>
        ///     raw H.261
        ///</summary>
        h261,

        ///<summary>
        ///     raw H.263
        ///</summary>
        h263,

        ///<summary>
        ///     raw H.264 video
        ///</summary>
        h264,

        ///<summary>
        ///     Hash testing
        ///</summary>
        hash,

        ///<summary>
        ///     Macintosh HCOM
        ///</summary>
        hcom,

        ///<summary>
        ///     HDS Muxer
        ///</summary>
        hds,

        ///<summary>
        ///     raw HEVC video
        ///</summary>
        hevc,

        ///<summary>
        ///     Apple HTTP Live Streaming
        ///</summary>
        hls,

        ///<summary>
        ///     Cryo HNM v4
        ///</summary>
        hnm,

        ///<summary>
        ///     Microsoft Windows ICO
        ///</summary>
        ico,

        ///<summary>
        ///     id Cinematic
        ///</summary>
        idcin,

        ///<summary>
        ///     iCE Draw File
        ///</summary>
        idf,

        ///<summary>
        ///     IFF (Interchange File Format)
        ///</summary>
        iff,

        ///<summary>
        ///     IFV CCTV DVR
        ///</summary>
        ifv,

        ///<summary>
        ///     iLBC storage
        ///</summary>
        ilbc,

        ///<summary>
        ///     image2 sequence
        ///</summary>
        image2,

        ///<summary>
        ///     piped image2 sequence
        ///</summary>
        image2pipe,

        ///<summary>
        ///     raw Ingenient MJPEG
        ///</summary>
        ingenient,

        ///<summary>
        ///     Interplay MVE
        ///</summary>
        ipmovie,

        ///<summary>
        ///     iPod H.264 MP4 (MPEG-4 Part 14)
        ///</summary>
        ipod,

        ///<summary>
        ///     Berkeley/IRCAM/CARL Sound Format
        ///</summary>
        ircam,

        ///<summary>
        ///     ISMV/ISMA (Smooth Streaming)
        ///</summary>
        ismv,

        ///<summary>
        ///     Funcom ISS
        ///</summary>
        iss,

        ///<summary>
        ///     IndigoVision 8000 video
        ///</summary>
        iv8,

        ///<summary>
        ///     On2 IVF
        ///</summary>
        ivf,

        ///<summary>
        ///     IVR (Internet Video Recording)
        ///</summary>
        ivr,

        ///<summary>
        ///     piped j2k sequence
        ///</summary>
        j2k_pipe,

        ///<summary>
        ///     JACOsub subtitle format
        ///</summary>
        jacosub,

        ///<summary>
        ///     piped jpeg sequence
        ///</summary>
        jpeg_pipe,

        ///<summary>
        ///     piped jpegls sequence
        ///</summary>
        jpegls_pipe,

        ///<summary>
        ///     Bitmap Brothers JV
        ///</summary>
        jv,

        ///<summary>
        ///     KUX (YouKu)
        ///</summary>
        kux,

        ///<summary>
        ///     LOAS/LATM
        ///</summary>
        latm,

        ///<summary>
        ///     Libavfilter virtual input device
        ///</summary>
        lavfi,

        ///<summary>
        ///     Tracker formats (libopenmpt)
        ///</summary>
        libopenmpt,

        ///<summary>
        ///     live RTMP FLV (Flash Video)
        ///</summary>
        live_flv,

        ///<summary>
        ///     raw lmlm4
        ///</summary>
        lmlm4,

        ///<summary>
        ///     LOAS AudioSyncStream
        ///</summary>
        loas,

        ///<summary>
        ///     LRC lyrics
        ///</summary>
        lrc,

        ///<summary>
        ///     LVF
        ///</summary>
        lvf,

        ///<summary>
        ///     VR native stream (LXF)
        ///</summary>
        lxf,

        ///<summary>
        ///     raw MPEG-4 video
        ///</summary>
        m4v,

        ///<summary>
        ///        Matroska / WebM
        ///</summary>
        matroska,

        ///<summary>
        ///     MD5 testing
        ///</summary>
        md5,

        ///<summary>
        ///     Metal Gear Solid: The Twin Snakes
        ///</summary>
        mgsts,

        ///<summary>
        ///     MicroDVD subtitle format
        ///</summary>
        microdvd,

        ///<summary>
        ///     raw MJPEG video
        ///</summary>
        mjpeg,

        ///<summary>
        ///     raw MJPEG 2000 video
        ///</summary>
        mjpeg_2000,

        ///<summary>
        ///      pts as timecode v2 format, as defined by mkvtoolnix
        ///</summary>
        mkvtimestamp_v2,

        ///<summary>
        ///     raw MLP
        ///</summary>
        mlp,

        ///<summary>
        ///     Magic Lantern Video (MLV)
        ///</summary>
        mlv,

        ///<summary>
        ///     American Laser Games MM
        ///</summary>
        mm,

        ///<summary>
        ///     Yamaha SMAF
        ///</summary>
        mmf,

        ///<summary>
        ///     ,m4a,3gp,3g2,mj2 QuickTime / MOV
        ///</summary>
        mov,

        ///<summary>
        ///     MP2 (MPEG audio layer 2)
        ///</summary>
        mp2,

        ///<summary>
        ///     MP3 (MPEG audio layer 3)
        ///</summary>
        mp3,

        ///<summary>
        ///     MP4 (MPEG-4 Part 14)
        ///</summary>
        mp4,

        ///<summary>
        ///     Musepack
        ///</summary>
        mpc,

        ///<summary>
        ///     Musepack SV8
        ///</summary>
        mpc8,

        ///<summary>
        ///     MPEG-1 Systems / MPEG program stream
        ///</summary>
        mpeg,

        ///<summary>
        ///     raw MPEG-1 video
        ///</summary>
        mpeg1video,

        ///<summary>
        ///     raw MPEG-2 video
        ///</summary>
        mpeg2video,

        ///<summary>
        ///     MPEG-TS (MPEG-2 Transport Stream)
        ///</summary>
        mpegts,

        ///<summary>
        ///     raw MPEG-TS (MPEG-2 Transport Stream)
        ///</summary>
        mpegtsraw,

        ///<summary>
        ///     raw MPEG video
        ///</summary>
        mpegvideo,

        ///<summary>
        ///     MIME multipart JPEG
        ///</summary>
        mpjpeg,

        ///<summary>
        ///     MPL2 subtitles
        ///</summary>
        mpl2,

        ///<summary>
        ///     MPlayer subtitles
        ///</summary>
        mpsub,

        ///<summary>
        ///     Sony PS3 MSF
        ///</summary>
        msf,

        ///<summary>
        ///     MSN TCP Webcam stream
        ///</summary>
        msnwctcp,

        ///<summary>
        ///     Konami PS2 MTAF
        ///</summary>
        mtaf,

        ///<summary>
        ///     MTV
        ///</summary>
        mtv,

        ///<summary>
        ///     PCM mu-law
        ///</summary>
        mulaw,

        ///<summary>
        ///     Eurocom MUSX
        ///</summary>
        musx,

        ///<summary>
        ///     Silicon Graphics Movie
        ///</summary>
        mv,

        ///<summary>
        ///     Motion Pixels MVI
        ///</summary>
        mvi,

        ///<summary>
        ///     MXF (Material eXchange Format)
        ///</summary>
        mxf,

        ///<summary>
        ///     MXF (Material eXchange Format) D-10 Mapping
        ///</summary>
        mxf_d10,

        ///<summary>
        ///     MXF (Material eXchange Format) Operational Pattern Atom
        ///</summary>
        mxf_opatom,

        ///<summary>
        ///     MxPEG clip
        ///</summary>
        mxg,

        ///<summary>
        ///     NC camera feed
        ///</summary>
        nc,

        ///<summary>
        ///     NIST SPeech HEader REsources
        ///</summary>
        nistsphere,

        ///<summary>
        ///     Computerized Speech Lab NSP
        ///</summary>
        nsp,

        ///<summary>
        ///     Nullsoft Streaming Video
        ///</summary>
        nsv,

        ///<summary>
        ///     NUT
        ///</summary>
        nut,

        ///<summary>
        ///     NuppelVideo
        ///</summary>
        nuv,

        ///<summary>
        ///     Ogg Audio
        ///</summary>
        oga,

        ///<summary>
        ///     Ogg
        ///</summary>
        ogg,

        ///<summary>
        ///     Ogg Video
        ///</summary>
        ogv,

        ///<summary>
        ///     Sony OpenMG audio
        ///</summary>
        oma,

        ///<summary>
        ///     Ogg Opus
        ///</summary>
        opus,

        ///<summary>
        ///     Amazing Studio Packed Animation File
        ///</summary>
        paf,

        ///<summary>
        ///     piped pam sequence
        ///</summary>
        pam_pipe,

        ///<summary>
        ///     piped pbm sequence
        ///</summary>
        pbm_pipe,

        ///<summary>
        ///     piped pcx sequence
        ///</summary>
        pcx_pipe,

        ///<summary>
        ///     piped pgm sequence
        ///</summary>
        pgm_pipe,

        ///<summary>
        ///     piped pgmyuv sequence
        ///</summary>
        pgmyuv_pipe,

        ///<summary>
        ///     piped pictor sequence
        ///</summary>
        pictor_pipe,

        ///<summary>
        ///     PJS (Phoenix Japanimation Society) subtitles
        ///</summary>
        pjs,

        ///<summary>
        ///     Playstation Portable PMP
        ///</summary>
        pmp,

        ///<summary>
        ///     piped png sequence
        ///</summary>
        png_pipe,

        ///<summary>
        ///     piped ppm sequence
        ///</summary>
        ppm_pipe,

        ///<summary>
        ///     piped psd sequence
        ///</summary>
        psd_pipe,

        ///<summary>
        ///     PSP MP4 (MPEG-4 Part 14)
        ///</summary>
        psp,

        ///<summary>
        ///     Sony Playstation STR
        ///</summary>
        psxstr,

        ///<summary>
        ///     TechnoTrend PVA
        ///</summary>
        pva,

        ///<summary>
        ///     PVF (Portable Voice Format)
        ///</summary>
        pvf,

        ///<summary>
        ///     QCP
        ///</summary>
        qcp,

        ///<summary>
        ///     piped qdraw sequence
        ///</summary>
        qdraw_pipe,

        ///<summary>
        ///     REDCODE R3D
        ///</summary>
        r3d,

        ///<summary>
        ///     raw video
        ///</summary>
        rawvideo,

        ///<summary>
        ///     RealText subtitle format
        ///</summary>
        realtext,

        ///<summary>
        ///     RedSpark
        ///</summary>
        redspark,

        ///<summary>
        ///     RL2
        ///</summary>
        rl2,

        ///<summary>
        ///     RealMedia
        ///</summary>
        rm,

        ///<summary>
        ///     raw id RoQ
        ///</summary>
        roq,

        ///<summary>
        ///     RPL / ARMovie
        ///</summary>
        rpl,

        ///<summary>
        ///     GameCube RSD
        ///</summary>
        rsd,

        ///<summary>
        ///     Lego Mindstorms RSO
        ///</summary>
        rso,

        ///<summary>
        ///     RTP output
        ///</summary>
        rtp,

        ///<summary>
        ///     RTP/mpegts output format
        ///</summary>
        rtp_mpegts,

        ///<summary>
        ///     RTSP output
        ///</summary>
        rtsp,

        ///<summary>
        ///     PCM signed 16-bit big-endian
        ///</summary>
        s16be,

        ///<summary>
        ///     PCM signed 16-bit little-endian
        ///</summary>
        s16le,

        ///<summary>
        ///     PCM signed 24-bit big-endian
        ///</summary>
        s24be,

        ///<summary>
        ///     PCM signed 24-bit little-endian
        ///</summary>
        s24le,

        ///<summary>
        ///     PCM signed 32-bit big-endian
        ///</summary>
        s32be,

        ///<summary>
        ///     PCM signed 32-bit little-endian
        ///</summary>
        s32le,

        ///<summary>
        ///     SMPTE 337M
        ///</summary>
        s337m,

        ///<summary>
        ///     PCM signed 8-bit
        ///</summary>
        s8,

        ///<summary>
        ///     SAMI subtitle format
        ///</summary>
        sami,

        ///<summary>
        ///     SAP output
        ///</summary>
        sap,

        ///<summary>
        ///     raw SBC
        ///</summary>
        sbc,

        ///<summary>
        ///     SBaGen binaural beats script
        ///</summary>
        sbg,

        ///<summary>
        ///     Scenarist Closed Captions
        ///</summary>
        scc,

        ///<summary>
        ///             SDL2 output device
        ///</summary>
        sdl,

        ///<summary>
        ///     SDP
        ///</summary>
        sdp,

        ///<summary>
        ///     SDR2
        ///</summary>
        sdr2,

        ///<summary>
        ///     MIDI Sample Dump Standard
        ///</summary>
        sds,

        ///<summary>
        ///     Sample Dump eXchange
        ///</summary>
        sdx,

        ///<summary>
        ///     segment
        ///</summary>
        segment,

        ///<summary>
        ///     SER (Simple uncompressed video format for astronomical capturing)
        ///</summary>
        ser,

        ///<summary>
        ///     piped sgi sequence
        ///</summary>
        sgi_pipe,

        ///<summary>
        ///     raw Shorten
        ///</summary>
        shn,

        ///<summary>
        ///     Beam Software SIFF
        ///</summary>
        siff,

        ///<summary>
        ///     JPEG single image
        ///</summary>
        singlejpeg,

        ///<summary>
        ///     Asterisk raw pcm
        ///</summary>
        sln,

        ///<summary>
        ///     Loki SDL MJPEG
        ///</summary>
        smjpeg,

        ///<summary>
        ///     Smacker
        ///</summary>
        smk,

        ///<summary>
        ///      Streaming Muxer
        ///</summary>
        smoothstreaming,

        ///<summary>
        ///     LucasArts Smush
        ///</summary>
        smush,

        ///<summary>
        ///     Sierra SOL
        ///</summary>
        sol,

        ///<summary>
        ///     SoX native
        ///</summary>
        sox,

        ///<summary>
        ///     IEC 61937 (used on S/PDIF - IEC958)
        ///</summary>
        spdif,

        ///<summary>
        ///     Ogg Speex
        ///</summary>
        spx,

        ///<summary>
        ///     SubRip subtitle
        ///</summary>
        srt,

        ///<summary>
        ///     Spruce subtitle format
        ///</summary>
        stl,

        ///<summary>
        ///      streaming segment muxer
        ///</summary>
        stream_segment,

        ///<summary>
        ///     SubViewer subtitle format
        ///</summary>
        subviewer,

        ///<summary>
        ///     SubViewer v1 subtitle format
        ///</summary>
        subviewer1,

        ///<summary>
        ///     piped sunrast sequence
        ///</summary>
        sunrast_pipe,

        ///<summary>
        ///     raw HDMV Presentation Graphic Stream subtitles
        ///</summary>
        sup,

        ///<summary>
        ///     Konami PS2 SVAG
        ///</summary>
        svag,

        ///<summary>
        ///     MPEG-2 PS (SVCD)
        ///</summary>
        svcd,

        ///<summary>
        ///     piped svg sequence
        ///</summary>
        svg_pipe,

        ///<summary>
        ///     SWF (ShockWave Flash)
        ///</summary>
        swf,

        ///<summary>
        ///     raw TAK
        ///</summary>
        tak,

        ///<summary>
        ///     TED Talks captions
        ///</summary>
        tedcaptions,

        ///<summary>
        ///     Multiple muxer tee
        ///</summary>
        tee,

        ///<summary>
        ///     THP
        ///</summary>
        thp,

        ///<summary>
        ///     Tiertex Limited SEQ
        ///</summary>
        tiertexseq,

        ///<summary>
        ///     piped tiff sequence
        ///</summary>
        tiff_pipe,

        ///<summary>
        ///     8088flex TMV
        ///</summary>
        tmv,

        ///<summary>
        ///     raw TrueHD
        ///</summary>
        truehd,

        ///<summary>
        ///     TTA (True Audio)
        ///</summary>
        tta,

        ///<summary>
        ///     Tele-typewriter
        ///</summary>
        tty,

        ///<summary>
        ///     Renderware TeXture Dictionary
        ///</summary>
        txd,

        ///<summary>
        ///     TiVo TY Stream
        ///</summary>
        ty,

        ///<summary>
        ///     PCM unsigned 16-bit big-endian
        ///</summary>
        u16be,

        ///<summary>
        ///     PCM unsigned 16-bit little-endian
        ///</summary>
        u16le,

        ///<summary>
        ///     PCM unsigned 24-bit big-endian
        ///</summary>
        u24be,

        ///<summary>
        ///     PCM unsigned 24-bit little-endian
        ///</summary>
        u24le,

        ///<summary>
        ///     PCM unsigned 32-bit big-endian
        ///</summary>
        u32be,

        ///<summary>
        ///     PCM unsigned 32-bit little-endian
        ///</summary>
        u32le,

        ///<summary>
        ///     PCM unsigned 8-bit
        ///</summary>
        u8,

        ///<summary>
        ///      framecrc testing
        ///</summary>
        uncodedframecrc,

        ///<summary>
        ///     Uncompressed 4:2:2 10-bit
        ///</summary>
        v210,

        ///<summary>
        ///     Uncompressed 4:2:2 10-bit
        ///</summary>
        v210x,

        ///<summary>
        ///     Sony PS2 VAG
        ///</summary>
        vag,

        ///<summary>
        ///     raw VC-1 video
        ///</summary>
        vc1,

        ///<summary>
        ///     VC-1 test bitstream
        ///</summary>
        vc1test,

        ///<summary>
        ///     MPEG-1 Systems / MPEG program stream (VCD)
        ///</summary>
        vcd,

        ///<summary>
        ///     VfW video capture
        ///</summary>
        vfwcap,

        ///<summary>
        ///     PCM Archimedes VIDC
        ///</summary>
        vidc,

        ///<summary>
        ///     Vividas VIV
        ///</summary>
        vividas,

        ///<summary>
        ///     Vivo
        ///</summary>
        vivo,

        ///<summary>
        ///     Sierra VMD
        ///</summary>
        vmd,

        ///<summary>
        ///     MPEG-2 PS (VOB)
        ///</summary>
        vob,

        ///<summary>
        ///     VobSub subtitle format
        ///</summary>
        vobsub,

        ///<summary>
        ///     Creative Voice
        ///</summary>
        voc,

        ///<summary>
        ///     Sony PS2 VPK
        ///</summary>
        vpk,

        ///<summary>
        ///     VPlayer subtitles
        ///</summary>
        vplayer,

        ///<summary>
        ///     Nippon Telegraph and Telephone Corporation (NTT) TwinVQ
        ///</summary>
        vqf,

        ///<summary>
        ///     Sony Wave64
        ///</summary>
        w64,

        ///<summary>
        ///     WAV / WAVE (Waveform Audio)
        ///</summary>
        wav,

        ///<summary>
        ///     Wing Commander III movie
        ///</summary>
        wc3movie,

        ///<summary>
        ///     WebM
        ///</summary>
        webm,

        ///<summary>
        ///     WebM Chunk Muxer
        ///</summary>
        webm_chunk,

        ///<summary>
        ///      DASH Manifest
        ///</summary>
        webm_dash_manifest,

        ///<summary>
        ///     WebP
        ///</summary>
        webp,

        ///<summary>
        ///     piped webp sequence
        ///</summary>
        webp_pipe,

        ///<summary>
        ///     WebVTT subtitle
        ///</summary>
        webvtt,

        ///<summary>
        ///     Westwood Studios audio
        ///</summary>
        wsaud,

        ///<summary>
        ///     Wideband Single-bit Data (WSD)
        ///</summary>
        wsd,

        ///<summary>
        ///     Westwood Studios VQA
        ///</summary>
        wsvqa,

        ///<summary>
        ///     Windows Television (WTV)
        ///</summary>
        wtv,

        ///<summary>
        ///     raw WavPack
        ///</summary>
        wv,

        ///<summary>
        ///     Psion 3 audio
        ///</summary>
        wve,

        ///<summary>
        ///     Maxis XA
        ///</summary>
        xa,

        ///<summary>
        ///     eXtended BINary text (XBIN)
        ///</summary>
        xbin,

        ///<summary>
        ///     Microsoft XMV
        ///</summary>
        xmv,

        ///<summary>
        ///     piped xpm sequence
        ///</summary>
        xpm_pipe,

        ///<summary>
        ///     Sony PS3 XVAG
        ///</summary>
        xvag,

        ///<summary>
        ///     piped xwd sequence
        ///</summary>
        xwd_pipe,

        ///<summary>
        ///     Microsoft xWMA
        ///</summary>
        xwma,

        ///<summary>
        ///     Psygnosis YOP
        ///</summary>
        yop,

        ///<summary>
        ///     YUV4MPEG pipe
        ///</summary>
        yuv4mpegpipe,

        ///<summary>
        ///     x11grab
        ///</summary>
        x11grab,

        ///<summary>
        ///         avfoundation
        ///</summary>
        avfoundation,

        ///<summary>
        ///         video4linux2
        ///</summary>
        v4l2
    }

    public enum Flag
    {
        ///<summary>
        /// Use four motion vector by macroblock (mpeg4).
        ///</summary>
        mv4,

        ///<summary>
        /// Use 1/4 pel motion compensation.
        ///</summary>
        qpel,

        ///<summary>
        /// Use loop filter.
        ///</summary>
        loop,

        ///<summary>
        /// Use fixed qscale.
        ///</summary>
        qscale,

        ///<summary>
        /// Use internal 2pass ratecontrol in first pass mode.
        ///</summary>
        pass1,

        ///<summary>
        /// Use internal 2pass ratecontrol in second pass mode.
        ///</summary>
        pass2,

        ///<summary>
        /// Only decode/encode grayscale.
        ///</summary>
        gray,

        ///<summary>
        /// Do not draw edges.
        ///</summary>
        emu_edge,

        ///<summary>
        /// Set error[?] variables during encoding.
        ///</summary>
        psnr,

        ///<summary>
        /// Input bitstream might be randomly truncated.
        ///</summary>
        truncated,

        ///<summary>
        /// Dont output frames whose parameters differ from first decoded frame in stream. Error AVERROR_INPUT_CHANGED is returned when a frame is dropped.
        ///</summary>
        drop_changed,

        ///<summary>
        /// Use interlaced DCT.
        ///</summary>
        ildct,

        ///<summary>
        /// Force low delay.
        ///</summary>
        low_delay,

        ///<summary>
        /// Place global headers in extradata instead of every keyframe.
        ///</summary>
        global_header,

        ///<summary>
        /// Only write platform-, build- and time-independent data. (except (I)DCT). This ensures that file and data checksums are reproducible and match between platforms. Its primary use is for regression testing.
        ///</summary>
        bitexact,

        ///<summary>
        /// Apply H263 advanced intra coding / mpeg4 ac prediction.
        ///</summary>
        aic,

        ///<summary>
        /// Deprecated, use mpegvideo private options instead.
        ///</summary>
        [Obsolete]
        cbp,

        ///<summary>
        /// Deprecated, use mpegvideo private options instead.
        ///</summary>
        [Obsolete]
        qprd,

        ///<summary>
        /// Apply interlaced motion estimation.
        ///</summary>
        ilme,

        ///<summary>
        /// Use closed gop.
        ///</summary>
        cgop,

        ///<summary>
        /// Output even potentially corrupted frames.output_corrupt
        ///</summary>
        output_corrupt
    }
    public enum Position
    {
        /// <summary>
        ///     Upper left corner
        /// </summary>
        UpperLeft,

        /// <summary>
        ///     Upper right corner
        /// </summary>
        UpperRight,

        /// <summary>
        ///     5px from right border
        /// </summary>
        Right,

        /// <summary>
        ///     5px from left border
        /// </summary>
        Left,

        /// <summary>
        ///     5x from upper border
        /// </summary>
        Up,

        /// <summary>
        ///     Bottom right corner
        /// </summary>
        BottomRight,

        /// <summary>
        ///     Bottom left corner
        /// </summary>
        BottomLeft,

        /// <summary>
        ///     Center of video
        /// </summary>
        Center,

        /// <summary>
        ///     5px from bottom corner
        /// </summary>
        Bottom
    }
    public enum BitstreamFilter
    {
        /// <summary>
        ///     Convert MPEG-2/4 AAC ADTS to an MPEG-4 Audio Specific Configuration bitstream.
        /// </summary>
        aac_adtstoasc,
        /// <summary>
        ///     Modify metadata embedded in an AV1 stream.
        /// </summary>
        av1_metadata,
        /// <summary>
        ///     Remove zero padding at the end of a packet.
        /// </summary>
        chomp,
        /// <summary>
        ///     Extract the core from a DCA/DTS stream, dropping extensions such as DTS-HD.
        /// </summary>
        dca_core,
        /// <summary>
        ///     Add extradata to the beginning of the filtered packets except when said packets already exactly begin with the extradata that is intended to be added.
        /// </summary>
        dump_extra,
        /// <summary>
        ///     Extract the core from a E-AC-3 stream, dropping extra channels.
        /// </summary>
        eac3_core,
        /// <summary>
        ///     Extract the in-band extradata.
        /// </summary>
        extract_extradata,
        /// <summary>
        ///     Remove units with types in or not in a given set from the stream.
        /// </summary>
        filter_units,
        /// <summary>
        ///     Extract Rgb or Alpha part of an HAPQA file, without recompression, in order to create an HAPQ or an HAPAlphaOnly file.
        /// </summary>
        hapqa_extract,
        /// <summary>
        ///     Modify metadata embedded in an H.264 stream.
        /// </summary>
        h264_metadata,
        /// <summary>
        ///     Convert an H.264 bitstream from length prefixed mode to start code prefixed mode (as defined in the Annex B of the ITU-T H.264 specification).
        /// </summary>
        h264_mp4toannexb,
        /// <summary>
        ///     This applies a specific fixup to some Blu-ray streams which contain redundant PPSs modifying irrelevant parameters of the stream which confuse other transformations which require correct extradata.
        /// </summary>
        h264_redundant_pps,
        /// <summary>
        ///     Modify metadata embedded in an HEVC stream.
        /// </summary>
        hevc_metadata,
        /// <summary>
        ///     Convert an HEVC/H.265 bitstream from length prefixed mode to start code prefixed mode (as defined in the Annex B of the ITU-T H.265 specification).
        /// </summary>
        hevc_mp4toannexb,
        /// <summary>
        ///     Modifies the bitstream to fit in MOV and to be usable by the Final Cut Pro decoder. This filter only applies to the mpeg2video codec, and is likely not needed for Final Cut Pro 7 and newer with the appropriate -tag:v.
        /// </summary>
        imxdump,
        /// <summary>
        ///     Convert MJPEG/AVI1 packets to full JPEG/JFIF packets.
        /// </summary>
        mjpeg2jpeg,
        /// <summary>
        ///     Add an MJPEG A header to the bitstream, to enable decoding by Quicktime.
        /// </summary>
        mjpegadump,
        /// <summary>
        ///     Extract a representable text file from MOV subtitles, stripping the metadata header from each subtitle packet.
        /// </summary>
        mov2textsub,
        /// <summary>
        ///     Decompress non-standard compressed MP3 audio headers.
        /// </summary>
        mp3decomp,
        /// <summary>
        ///     Modify metadata embedded in an MPEG-2 stream.
        /// </summary>
        mpeg2_metadata,
        /// <summary>
        ///     Unpack DivX-style packed B-frames.
        /// </summary>
        mpeg4_unpack_bframes,
        /// <summary>
        ///     Damages the contents of packets or simply drops them without damaging the container. Can be used for fuzzing or testing error resilience/concealment.
        /// </summary>
        noise,
        /// <summary>
        ///     Modify color property metadata embedded in prores stream.
        /// </summary>
        prores_metadata,
        /// <summary>
        ///     Remove extradata from packets.
        /// </summary>
        remove_extra,
        /// <summary>
        ///     Convert text subtitles to MOV subtitles (as used by the mov_text codec) with metadata headers.
        /// </summary>
        text2movsub,
        /// <summary>
        ///     Log trace output containing all syntax elements in the coded stream headers (everything above the level of individual coded blocks). This can be useful for debugging low-level stream issues.
        /// </summary>
        trace_headers,
        /// <summary>
        ///     Extract the core from a TrueHD stream, dropping ATMOS data.
        /// </summary>
        truehd_core,
        /// <summary>
        ///     Modify metadata embedded in a VP9 stream.
        /// </summary>
        vp9_metadata,
        /// <summary>
        ///     Merge VP9 invisible (alt-ref) frames back into VP9 superframes. This fixes merging of split/segmented VP9 streams where the alt-ref frame was split from its visible counterpart.
        /// </summary>
        vp9_superframe,
        /// <summary>
        ///     Split VP9 superframes into single frames.
        /// </summary>
        vp9_superframe_split,
        /// <summary>
        ///     Given a VP9 stream with correct timestamps but possibly out of order, insert additional show-existing-frame packets to correct the ordering.
        /// </summary>
        vp9_raw_reorder
    }
    public enum VideoSize
    {
        /// <summary>
        ///     720x480
        /// </summary>
        Ntsc,

        /// <summary>
        ///     720x576
        /// </summary>
        Pal,

        /// <summary>
        ///     352x240
        /// </summary>
        Qntsc,

        /// <summary>
        ///     352x288
        /// </summary>
        Qpal,

        /// <summary>
        ///     640x480
        /// </summary>
        Sntsc,

        /// <summary>
        ///     768x576
        /// </summary>
        Spal,

        /// <summary>
        ///     352x240
        /// </summary>
        Film,

        /// <summary>
        ///     352x240
        /// </summary>
        NtscFilm,

        /// <summary>
        ///     128x96
        /// </summary>
        Sqcif,

        /// <summary>
        ///     176x144
        /// </summary>
        Qcif,

        /// <summary>
        ///     352x288
        /// </summary>
        Cif,

        /// <summary>
        ///     704x576
        /// </summary>
        _4Cif,

        /// <summary>
        ///     1408x1152
        /// </summary>
        _16cif,

        /// <summary>
        ///     160x120
        /// </summary>
        Qqvga,

        /// <summary>
        ///     320x240
        /// </summary>
        Qvga,

        /// <summary>
        ///     640x480
        /// </summary>
        Vga,

        /// <summary>
        ///     800x600
        /// </summary>
        Svga,

        /// <summary>
        ///     1024x768
        /// </summary>
        Xga,

        /// <summary>
        ///     1600x1200
        /// </summary>
        Uxga,

        /// <summary>
        ///     2048x1536
        /// </summary>
        Qxga,

        /// <summary>
        ///     1280x1024
        /// </summary>
        Sxga,

        /// <summary>
        ///     2560x2048
        /// </summary>
        Qsxga,

        /// <summary>
        ///     5120x4096
        /// </summary>
        Hsxga,

        /// <summary>
        ///     852x480
        /// </summary>
        Wvga,

        /// <summary>
        ///     1366x768
        /// </summary>
        Wxga,

        /// <summary>
        ///     1600x1024
        /// </summary>
        Wsxga,

        /// <summary>
        ///     1920x1200
        /// </summary>
        Wuxga,

        /// <summary>
        ///     2560x1600
        /// </summary>
        Woxga,

        /// <summary>
        ///     3200x2048
        /// </summary>
        Wqsxga,

        /// <summary>
        ///     3840x2400
        /// </summary>
        Wquxga,

        /// <summary>
        ///     6400x4096
        /// </summary>
        Whsxga,

        /// <summary>
        ///     7680x4800
        /// </summary>
        Whuxga,

        /// <summary>
        ///     320x200
        /// </summary>
        Cga,

        /// <summary>
        ///     640x350
        /// </summary>
        Ega,

        /// <summary>
        ///     852x480
        /// </summary>
        Hd480,

        /// <summary>
        ///     1280x720
        /// </summary>
        Hd720,

        /// <summary>
        ///     1920x1080
        /// </summary>
        Hd1080,

        /// <summary>
        ///     2048x1080
        /// </summary>
        _2K,

        /// <summary>
        ///     1998x1080
        /// </summary>
        _2Kflat,

        /// <summary>
        ///     2048x858
        /// </summary>
        _2Kscope,

        /// <summary>
        ///     4096x2160
        /// </summary>
        _4K,

        /// <summary>
        ///     3996x2160
        /// </summary>
        _4Kflat,

        /// <summary>
        ///     4096x1716
        /// </summary>
        _4Kscope,

        /// <summary>
        ///     640x360
        /// </summary>
        Nhd,

        /// <summary>
        ///     240x160
        /// </summary>
        Hqvga,

        /// <summary>
        ///     400x240
        /// </summary>
        Wqvga,

        /// <summary>
        ///     432x240
        /// </summary>
        Fwqvga,

        /// <summary>
        ///     480x320
        /// </summary>
        Hvga,

        /// <summary>
        ///     960x540
        /// </summary>
        Qhd,

        /// <summary>
        ///     2048x1080
        /// </summary>
        _2Kdci,

        /// <summary>
        ///     4096x2160
        /// </summary>
        _4Kdci,

        /// <summary>
        ///     3840x2160
        /// </summary>
        Uhd2160,

        /// <summary>
        ///     7680x4320
        /// </summary>
        Uhd4320
    }
    public enum RotateDegrees
    {
        /// <summary>
        ///     90 degrees counter clockwise and vertical flip
        /// </summary>
        CounterClockwiseAndFlip = 0,

        /// <summary>
        ///     90 degress clockwise
        /// </summary>
        Clockwise = 1,

        /// <summary>
        ///     90 degrees counter clockwise
        /// </summary>
        CounterClockwise = 2,

        /// <summary>
        ///     90 degrees counter clockwise and vertical flip
        /// </summary>
        ClockwiseAndFlip = 3,

        /// <summary>
        ///     Rotate video by 180 degrees
        /// </summary>
        Invert = 5
    }
    public enum VideoCodec
    {
        ///<summary>
        ///      Uncompressed 4:2:2 10-bit
        ///</summary>
        _012v,

        ///<summary>
        ///      4X Movie
        ///</summary>
        _4xm,

        ///<summary>
        ///      QuickTime 8BPS video
        ///</summary>
        _8bps,

        ///<summary>
        ///      Multicolor charset for Commodore 64 (encoders: a64multi )
        ///</summary>
        a64_multi,

        ///<summary>
        ///      Multicolor charset for Commodore 64, extended with 5th color (colram) (encoders: a64multi5 )
        ///</summary>
        a64_multi5,

        ///<summary>
        ///      Autodesk RLE
        ///</summary>
        aasc,

        ///<summary>
        ///      Amuse Graphics Movie
        ///</summary>
        agm,

        ///<summary>
        ///      Apple Intermediate Codec
        ///</summary>
        aic,

        ///<summary>
        ///      Alias/Wavefront PIX image
        ///</summary>
        alias_pix,

        ///<summary>
        ///      AMV Video
        ///</summary>
        amv,

        ///<summary>
        ///      Deluxe Paint Animation
        ///</summary>
        anm,

        ///<summary>
        ///      ASCII/ANSI art
        ///</summary>
        ansi,

        ///<summary>
        ///      APNG (Animated Portable Network Graphics) image
        ///</summary>
        apng,

        ///<summary>
        ///      Gryphon's Anim Compressor
        ///</summary>
        arbc,

        ///<summary>
        ///      ASUS V1
        ///</summary>
        asv1,

        ///<summary>
        ///      ASUS V2
        ///</summary>
        asv2,

        ///<summary>
        ///      Auravision AURA
        ///</summary>
        aura,

        ///<summary>
        ///      Auravision Aura 2
        ///</summary>
        aura2,

        ///<summary>
        ///      Alliance for Open Media AV1 (decoders: libaom-av1 libdav1d ) (encoders: libaom-av1 )
        ///</summary>
        av1,

        ///<summary>
        ///      Avid AVI Codec
        ///</summary>
        avrn,

        ///<summary>
        ///      Avid 1:1 10-bit RGB Packer
        ///</summary>
        avrp,

        ///<summary>
        ///      AVS (Audio Video Standard) video
        ///</summary>
        avs,

        ///<summary>
        ///      AVS2-P2/IEEE1857.4
        ///</summary>
        avs2,

        ///<summary>
        ///      Avid Meridien Uncompressed
        ///</summary>
        avui,

        ///<summary>
        ///      Uncompressed packed MS 4:4:4:4
        ///</summary>
        ayuv,

        ///<summary>
        ///      Bethesda VID video
        ///</summary>
        bethsoftvid,

        ///<summary>
        ///      Brute Force and Ignorance
        ///</summary>
        bfi,

        ///<summary>
        ///      Bink video
        ///</summary>
        binkvideo,

        ///<summary>
        ///      Binary text
        ///</summary>
        bintext,

        ///<summary>
        ///      Bitpacked
        ///</summary>
        bitpacked,

        ///<summary>
        ///      BMP (Windows and OS/2 bitmap)
        ///</summary>
        bmp,

        ///<summary>
        ///      Discworld II BMV video
        ///</summary>
        bmv_video,

        ///<summary>
        ///      BRender PIX image
        ///</summary>
        brender_pix,

        ///<summary>
        ///      Interplay C93
        ///</summary>
        c93,

        ///<summary>
        ///      Chinese AVS (Audio Video Standard) (AVS1-P2, JiZhun profile)
        ///</summary>
        cavs,

        ///<summary>
        ///      CD Graphics video
        ///</summary>
        cdgraphics,

        ///<summary>
        ///      Commodore CDXL video
        ///</summary>
        cdxl,

        ///<summary>
        ///      Cineform HD
        ///</summary>
        cfhd,

        ///<summary>
        ///      Cinepak
        ///</summary>
        cinepak,

        ///<summary>
        ///      Iterated Systems ClearVideo
        ///</summary>
        clearvideo,

        ///<summary>
        ///      Cirrus Logic AccuPak
        ///</summary>
        cljr,

        ///<summary>
        ///      Canopus Lossless Codec
        ///</summary>
        cllc,

        ///<summary>
        ///      Electronic Arts CMV video (decoders: eacmv )
        ///</summary>
        cmv,

        ///<summary>
        ///      CPiA video format
        ///</summary>
        cpia,

        ///<summary>
        ///      CamStudio (decoders: camstudio )
        ///</summary>
        cscd,

        ///<summary>
        ///      Creative YUV (CYUV)
        ///</summary>
        cyuv,

        ///<summary>
        ///      Daala
        ///</summary>
        daala,

        ///<summary>
        ///      DirectDraw Surface image decoder
        ///</summary>
        dds,

        ///<summary>
        ///      Chronomaster DFA
        ///</summary>
        dfa,

        ///<summary>
        ///      Dirac (encoders: vc2 )
        ///</summary>
        dirac,

        ///<summary>
        ///      VC3/DNxHD
        ///</summary>
        dnxhd,

        ///<summary>
        ///      DPX (Digital Picture Exchange) image
        ///</summary>
        dpx,

        ///<summary>
        ///      Delphine Software International CIN video
        ///</summary>
        dsicinvideo,

        ///<summary>
        ///      DV (Digital Video)
        ///</summary>
        dvvideo,

        ///<summary>
        ///      Feeble Files/ScummVM DXA
        ///</summary>
        dxa,

        ///<summary>
        ///      Dxtory
        ///</summary>
        dxtory,

        ///<summary>
        ///      Resolume DXV
        ///</summary>
        dxv,

        ///<summary>
        ///      Escape 124
        ///</summary>
        escape124,

        ///<summary>
        ///      Escape 130
        ///</summary>
        escape130,

        ///<summary>
        ///      OpenEXR image
        ///</summary>
        exr,

        ///<summary>
        ///      FFmpeg video codec #1
        ///</summary>
        ffv1,

        ///<summary>
        ///      Huffyuv FFmpeg variant
        ///</summary>
        ffvhuff,

        ///<summary>
        ///      Mirillis FIC
        ///</summary>
        fic,

        ///<summary>
        ///      FITS (Flexible Image Transport System)
        ///</summary>
        fits,

        ///<summary>
        ///      Flash Screen Video v1
        ///</summary>
        flashsv,

        ///<summary>
        ///      Flash Screen Video v2
        ///</summary>
        flashsv2,

        ///<summary>
        ///      Autodesk Animator Flic video
        ///</summary>
        flic,

        ///<summary>
        ///      FLV / Sorenson Spark / Sorenson H.263 (Flash Video) (decoders: flv ) (encoders: flv )
        ///</summary>
        flv1,

        ///<summary>
        ///      FM Screen Capture Codec
        ///</summary>
        fmvc,

        ///<summary>
        ///      Fraps
        ///</summary>
        fraps,

        ///<summary>
        ///      Forward Uncompressed
        ///</summary>
        frwu,

        ///<summary>
        ///      Go2Meeting
        ///</summary>
        g2m,

        ///<summary>
        ///      Gremlin Digital Video
        ///</summary>
        gdv,

        ///<summary>
        ///      CompuServe GIF (Graphics Interchange Format)
        ///</summary>
        gif,

        ///<summary>
        ///      H.261
        ///</summary>
        h261,

        ///<summary>
        ///      H.263 / H.263-1996, H.263+ / H.263-1998 / H.263 version 2
        ///</summary>
        h263,

        ///<summary>
        ///      Intel H.263
        ///</summary>
        h263i,

        ///<summary>
        ///      H.263+ / H.263-1998 / H.263 version 2
        ///</summary>
        h263p,

        ///<summary>
        ///      H.264 / AVC / MPEG-4 AVC / MPEG-4 part 10 (decoders: h264 h264_qsv h264_cuvid ) (encoders: libx264 libx264rgb h264_amf h264_nvenc h264_qsv nvenc nvenc_h264 )
        ///</summary>
        h264,

        ///<summary>
        ///      Vidvox Hap
        ///</summary>
        hap,

        ///<summary>
        ///      H.265 / HEVC (High Efficiency Video Coding) (decoders: hevc hevc_qsv hevc_cuvid ) (encoders: libx265 nvenc_hevc hevc_amf hevc_nvenc hevc_qsv )
        ///</summary>
        hevc,

        ///<summary>
        ///      HNM 4 video
        ///</summary>
        hnm4video,

        ///<summary>
        ///      Canopus HQ/HQA
        ///</summary>
        hq_hqa,

        ///<summary>
        ///      Canopus HQX
        ///</summary>
        hqx,

        ///<summary>
        ///      HuffYUV
        ///</summary>
        huffyuv,

        ///<summary>
        ///      HuffYUV MT
        ///</summary>
        hymt,

        ///<summary>
        ///      id Quake II CIN video (decoders: idcinvideo )
        ///</summary>
        idcin,

        ///<summary>
        ///      iCEDraw text
        ///</summary>
        idf,

        ///<summary>
        ///      IFF ACBM/ANIM/DEEP/ILBM/PBM/RGB8/RGBN (decoders: iff )
        ///</summary>
        iff_ilbm,

        ///<summary>
        ///      Infinity IMM4
        ///</summary>
        imm4,

        ///<summary>
        ///      Intel Indeo 2
        ///</summary>
        indeo2,

        ///<summary>
        ///      Intel Indeo 3
        ///</summary>
        indeo3,

        ///<summary>
        ///      Intel Indeo Video Interactive 4
        ///</summary>
        indeo4,

        ///<summary>
        ///      Intel Indeo Video Interactive 5
        ///</summary>
        indeo5,

        ///<summary>
        ///      Interplay MVE video
        ///</summary>
        interplayvideo,

        ///<summary>
        ///      JPEG 2000 (decoders: jpeg2000 libopenjpeg ) (encoders: jpeg2000 libopenjpeg )
        ///</summary>
        jpeg2000,

        ///<summary>
        ///      JPEG-LS
        ///</summary>
        jpegls,

        ///<summary>
        ///      Bitmap Brothers JV video
        ///</summary>
        jv,

        ///<summary>
        ///      Kega Game Video
        ///</summary>
        kgv1,

        ///<summary>
        ///      Karl Morton's video codec
        ///</summary>
        kmvc,

        ///<summary>
        ///      Lagarith lossless
        ///</summary>
        lagarith,

        ///<summary>
        ///      Lossless JPEG
        ///</summary>
        ljpeg,

        ///<summary>
        ///      LOCO
        ///</summary>
        loco,

        ///<summary>
        ///      LEAD Screen Capture
        ///</summary>
        lscr,

        ///<summary>
        ///      Matrox Uncompressed SD
        ///</summary>
        m101,

        ///<summary>
        ///      Electronic Arts Madcow Video (decoders: eamad )
        ///</summary>
        mad,

        ///<summary>
        ///      MagicYUV video
        ///</summary>
        magicyuv,

        ///<summary>
        ///      Sony PlayStation MDEC (Motion DECoder)
        ///</summary>
        mdec,

        ///<summary>
        ///      Mimic
        ///</summary>
        mimic,

        ///<summary>
        ///      Motion JPEG (decoders: mjpeg mjpeg_cuvid ) (encoders: mjpeg mjpeg_qsv )
        ///</summary>
        mjpeg,

        ///<summary>
        ///      Apple MJPEG-B
        ///</summary>
        mjpegb,

        ///<summary>
        ///      American Laser Games MM Video
        ///</summary>
        mmvideo,

        ///<summary>
        ///      Motion Pixels video
        ///</summary>
        motionpixels,

        ///<summary>
        ///      MPEG-1 video (decoders: mpeg1video mpeg1_cuvid )
        ///</summary>
        mpeg1video,

        ///<summary>
        ///      MPEG-2 video (decoders: mpeg2video mpegvideo mpeg2_qsv mpeg2_cuvid ) (encoders: mpeg2video mpeg2_qsv )
        ///</summary>
        mpeg2video,

        ///<summary>
        ///      MPEG-4 part 2 (decoders: mpeg4 mpeg4_cuvid ) (encoders: mpeg4 libxvid )
        ///</summary>
        mpeg4,

        ///<summary>
        ///      MS ATC Screen
        ///</summary>
        msa1,

        ///<summary>
        ///      Mandsoft Screen Capture Codec
        ///</summary>
        mscc,

        ///<summary>
        ///      MPEG-4 part 2 Microsoft variant version 1
        ///</summary>
        msmpeg4v1,

        ///<summary>
        ///      MPEG-4 part 2 Microsoft variant version 2
        ///</summary>
        msmpeg4v2,

        ///<summary>
        ///      MPEG-4 part 2 Microsoft variant version 3 (decoders: msmpeg4 ) (encoders: msmpeg4 )
        ///</summary>
        msmpeg4v3,

        ///<summary>
        ///      Microsoft RLE
        ///</summary>
        msrle,

        ///<summary>
        ///      MS Screen 1
        ///</summary>
        mss1,

        ///<summary>
        ///      MS Windows Media Video V9 Screen
        ///</summary>
        mss2,

        ///<summary>
        ///      Microsoft Video 1
        ///</summary>
        msvideo1,

        ///<summary>
        ///      LCL (LossLess Codec Library) MSZH
        ///</summary>
        mszh,

        ///<summary>
        ///      MS Expression Encoder Screen
        ///</summary>
        mts2,

        ///<summary>
        ///      Silicon Graphics Motion Video Compressor 1
        ///</summary>
        mvc1,

        ///<summary>
        ///      Silicon Graphics Motion Video Compressor 2
        ///</summary>
        mvc2,

        ///<summary>
        ///      MatchWare Screen Capture Codec
        ///</summary>
        mwsc,

        ///<summary>
        ///      Mobotix MxPEG video
        ///</summary>
        mxpeg,

        ///<summary>
        ///      NuppelVideo/RTJPEG
        ///</summary>
        nuv,

        ///<summary>
        ///      Amazing Studio Packed Animation File Video
        ///</summary>
        paf_video,

        ///<summary>
        ///      PAM (Portable AnyMap) image
        ///</summary>
        pam,

        ///<summary>
        ///      PBM (Portable BitMap) image
        ///</summary>
        pbm,

        ///<summary>
        ///      PC Paintbrush PCX image
        ///</summary>
        pcx,

        ///<summary>
        ///      PGM (Portable GrayMap) image
        ///</summary>
        pgm,

        ///<summary>
        ///      PGMYUV (Portable GrayMap YUV) image
        ///</summary>
        pgmyuv,

        ///<summary>
        ///      Pictor/PC Paint
        ///</summary>
        pictor,

        ///<summary>
        ///      Apple Pixlet
        ///</summary>
        pixlet,

        ///<summary>
        ///      PNG (Portable Network Graphics) image
        ///</summary>
        png,

        ///<summary>
        ///      PPM (Portable PixelMap) image
        ///</summary>
        ppm,

        ///<summary>
        ///      Apple ProRes (iCodec Pro) (encoders: prores prores_aw prores_ks )
        ///</summary>
        prores,

        ///<summary>
        ///      Brooktree ProSumer Video
        ///</summary>
        prosumer,

        ///<summary>
        ///      Photoshop PSD file
        ///</summary>
        psd,

        ///<summary>
        ///      V.Flash PTX image
        ///</summary>
        ptx,

        ///<summary>
        ///      Apple QuickDraw
        ///</summary>
        qdraw,

        ///<summary>
        ///      Q-team QPEG
        ///</summary>
        qpeg,

        ///<summary>
        ///      QuickTime Animation (RLE) video
        ///</summary>
        qtrle,

        ///<summary>
        ///      AJA Kona 10-bit RGB Codec
        ///</summary>
        r10k,

        ///<summary>
        ///      Uncompressed RGB 10-bit
        ///</summary>
        r210,

        ///<summary>
        ///      RemotelyAnywhere Screen Capture
        ///</summary>
        rasc,

        ///<summary>
        ///      raw video
        ///</summary>
        rawvideo,

        ///<summary>
        ///      RL2 video
        ///</summary>
        rl2,

        ///<summary>
        ///      id RoQ video (decoders: roqvideo ) (encoders: roqvideo )
        ///</summary>
        roq,

        ///<summary>
        ///      QuickTime video (RPZA)
        ///</summary>
        rpza,

        ///<summary>
        ///      innoHeim/Rsupport Screen Capture Codec
        ///</summary>
        rscc,

        ///<summary>
        ///      RealVideo 1.0
        ///</summary>
        rv10,

        ///<summary>
        ///      RealVideo 2.0
        ///</summary>
        rv20,

        ///<summary>
        ///      RealVideo 3.0
        ///</summary>
        rv30,

        ///<summary>
        ///      RealVideo 4.0
        ///</summary>
        rv40,

        ///<summary>
        ///      LucasArts SANM/SMUSH video
        ///</summary>
        sanm,

        ///<summary>
        ///      ScreenPressor
        ///</summary>
        scpr,

        ///<summary>
        ///      Screenpresso
        ///</summary>
        screenpresso,

        ///<summary>
        ///      SGI image
        ///</summary>
        sgi,

        ///<summary>
        ///      SGI RLE 8-bit
        ///</summary>
        sgirle,

        ///<summary>
        ///      BitJazz SheerVideo
        ///</summary>
        sheervideo,

        ///<summary>
        ///      Smacker video (decoders: smackvid )
        ///</summary>
        smackvideo,

        ///<summary>
        ///      QuickTime Graphics (SMC)
        ///</summary>
        smc,

        ///<summary>
        ///      Sigmatel Motion Video
        ///</summary>
        smvjpeg,

        ///<summary>
        ///      Snow
        ///</summary>
        snow,

        ///<summary>
        ///      Sunplus JPEG (SP5X)
        ///</summary>
        sp5x,

        ///<summary>
        ///      NewTek SpeedHQ
        ///</summary>
        speedhq,

        ///<summary>
        ///      Screen Recorder Gold Codec
        ///</summary>
        srgc,

        ///<summary>
        ///      Sun Rasterfile image
        ///</summary>
        sunrast,

        ///<summary>
        ///      Scalable Vector Graphics
        ///</summary>
        svg,

        ///<summary>
        ///      Sorenson Vector Quantizer 1 / Sorenson Video 1 / SVQ1
        ///</summary>
        svq1,

        ///<summary>
        ///      Sorenson Vector Quantizer 3 / Sorenson Video 3 / SVQ3
        ///</summary>
        svq3,

        ///<summary>
        ///      Truevision Targa image
        ///</summary>
        targa,

        ///<summary>
        ///      Pinnacle TARGA CineWave YUV16
        ///</summary>
        targa_y216,

        ///<summary>
        ///      TDSC
        ///</summary>
        tdsc,

        ///<summary>
        ///      Electronic Arts TGQ video (decoders: eatgq )
        ///</summary>
        tgq,

        ///<summary>
        ///      Electronic Arts TGV video (decoders: eatgv )
        ///</summary>
        tgv,

        ///<summary>
        ///      Theora (encoders: libtheora )
        ///</summary>
        theora,

        ///<summary>
        ///      Nintendo Gamecube THP video
        ///</summary>
        thp,

        ///<summary>
        ///      Tiertex Limited SEQ video
        ///</summary>
        tiertexseqvideo,

        ///<summary>
        ///      TIFF image
        ///</summary>
        tiff,

        ///<summary>
        ///      8088flex TMV
        ///</summary>
        tmv,

        ///<summary>
        ///      Electronic Arts TQI video (decoders: eatqi )
        ///</summary>
        tqi,

        ///<summary>
        ///      Duck TrueMotion 1.0
        ///</summary>
        truemotion1,

        ///<summary>
        ///      Duck TrueMotion 2.0
        ///</summary>
        truemotion2,

        ///<summary>
        ///      Duck TrueMotion 2.0 Real Time
        ///</summary>
        truemotion2rt,

        ///<summary>
        ///      TechSmith Screen Capture Codec (decoders: camtasia )
        ///</summary>
        tscc,

        ///<summary>
        ///      TechSmith Screen Codec 2
        ///</summary>
        tscc2,

        ///<summary>
        ///      Renderware TXD (TeXture Dictionary) image
        ///</summary>
        txd,

        ///<summary>
        ///      IBM UltiMotion (decoders: ultimotion )
        ///</summary>
        ulti,

        ///<summary>
        ///      Ut Video
        ///</summary>
        utvideo,

        ///<summary>
        ///      Uncompressed 4:2:2 10-bit
        ///</summary>
        v210,

        ///<summary>
        ///      Uncompressed 4:2:2 10-bit
        ///</summary>
        v210x,

        ///<summary>
        ///      Uncompressed packed 4:4:4
        ///</summary>
        v308,

        ///<summary>
        ///      Uncompressed packed QT 4:4:4:4
        ///</summary>
        v408,

        ///<summary>
        ///      Uncompressed 4:4:4 10-bit
        ///</summary>
        v410,

        ///<summary>
        ///      Beam Software VB
        ///</summary>
        vb,

        ///<summary>
        ///      VBLE Lossless Codec
        ///</summary>
        vble,

        ///<summary>
        ///      SMPTE VC-1 (decoders: vc1 vc1_qsv vc1_cuvid )
        ///</summary>
        vc1,

        ///<summary>
        ///      Windows Media Video 9 Image v2
        ///</summary>
        vc1image,

        ///<summary>
        ///      ATI VCR1
        ///</summary>
        vcr1,

        ///<summary>
        ///      Miro VideoXL (decoders: xl )
        ///</summary>
        vixl,

        ///<summary>
        ///      Sierra VMD video
        ///</summary>
        vmdvideo,

        ///<summary>
        ///      VMware Screen Codec / VMware Video
        ///</summary>
        vmnc,

        ///<summary>
        ///      On2 VP3
        ///</summary>
        vp3,

        ///<summary>
        ///      On2 VP4
        ///</summary>
        vp4,

        ///<summary>
        ///      On2 VP5
        ///</summary>
        vp5,

        ///<summary>
        ///      On2 VP6
        ///</summary>
        vp6,

        ///<summary>
        ///      On2 VP6 (Flash version, with alpha channel)
        ///</summary>
        vp6a,

        ///<summary>
        ///      On2 VP6 (Flash version)
        ///</summary>
        vp6f,

        ///<summary>
        ///      On2 VP7
        ///</summary>
        vp7,

        ///<summary>
        ///      On2 VP8 (decoders: vp8 libvpx vp8_cuvid vp8_qsv ) (encoders: libvpx )
        ///</summary>
        vp8,

        ///<summary>
        ///      Google VP9 (decoders: vp9 libvpx-vp9 vp9_cuvid ) (encoders: libvpx-vp9 )
        ///</summary>
        vp9,

        ///<summary>
        ///      WinCAM Motion Video
        ///</summary>
        wcmv,

        ///<summary>
        ///      WebP (encoders: libwebp_anim libwebp )
        ///</summary>
        webp,

        ///<summary>
        ///      Windows Media Video 7
        ///</summary>
        wmv1,

        ///<summary>
        ///      Windows Media Video 8
        ///</summary>
        wmv2,

        ///<summary>
        ///      Windows Media Video 9
        ///</summary>
        wmv3,

        ///<summary>
        ///      Windows Media Video 9 Image
        ///</summary>
        wmv3image,

        ///<summary>
        ///      Winnov WNV1
        ///</summary>
        wnv1,

        ///<summary>
        ///      AVFrame to AVPacket passthrough
        ///</summary>
        wrapped_avframe,

        ///<summary>
        ///      Westwood Studios VQA (Vector Quantized Animation) video (decoders: vqavideo )
        ///</summary>
        ws_vqa,

        ///<summary>
        ///      Wing Commander III / Xan
        ///</summary>
        xan_wc3,

        ///<summary>
        ///      Wing Commander IV / Xxan
        ///</summary>
        xan_wc4,

        ///<summary>
        ///      eXtended BINary text
        ///</summary>
        xbin,

        ///<summary>
        ///      XBM (X BitMap) image
        ///</summary>
        xbm,

        ///<summary>
        ///      X-face image
        ///</summary>
        xface,

        ///<summary>
        ///      XPM (X PixMap) image
        ///</summary>
        xpm,

        ///<summary>
        ///      XWD (X Window Dump) image
        ///</summary>
        xwd,

        ///<summary>
        ///      Uncompressed YUV 4:1:1 12-bit
        ///</summary>
        y41p,

        ///<summary>
        ///      YUY2 Lossless Codec
        ///</summary>
        ylc,

        ///<summary>
        ///      Psygnosis YOP Video
        ///</summary>
        yop,

        ///<summary>
        ///      Uncompressed packed 4:2:0
        ///</summary>
        yuv4,

        ///<summary>
        ///      ZeroCodec Lossless Video
        ///</summary>
        zerocodec,

        ///<summary>
        ///      LCL (LossLess Codec Library) ZLIB
        ///</summary>
        zlib,

        ///<summary>
        ///      Zip Motion Blocks Video
        ///</summary>
        zmbv,

        ///<summary>
        ///      copy
        ///</summary>
        copy,

        ///<summary>
        ///      h264_nvenc
        ///</summary>
        h264_nvenc,

        ///<summary>
        ///      h264_cuvid
        ///</summary>
        h264_cuvid,

        ///<summary>
        ///      H.265 / HEVC (High Efficiency Video Coding) (decoders: hevc hevc_qsv hevc_cuvid ) (encoders: libx265 nvenc_hevc hevc_amf hevc_nvenc hevc_qsv )
        ///</summary>
        hevc_amf,

        h264_amf,

        libxvid,

        libx265,

        ///<summary>
        ///      H.265 / HEVC (High Efficiency Video Coding) (decoders: hevc hevc_qsv hevc_cuvid ) (encoders: libx265 nvenc_hevc hevc_amf hevc_nvenc hevc_qsv )
        ///</summary>
        hevc_nvenc,

        ///<summary>
        ///      libx264
        ///</summary>
        libx264,

        ///<summary>
        ///      hevc_qsv (intel quicksync)
        ///</summary>
        hevc_qsv
    }

    public enum VideoSyncMethod
    {
        passthrough,
        cfr,
        vfr,
        drop,
        auto
    }

    public enum VsyncParams
    {
        passthrough,
        cfr,
        vfr,
        drop,
        auto
    }

    public enum ConversionPreset
    {
        VerySlow, Slower, Slow, Medium,
        Fast, Faster, VeryFast, SuperFast, UltraFast
    }
}
